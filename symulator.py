from azure.data.tables import TableServiceClient, UpdateMode
from azure.iot.device import IoTHubDeviceClient, Message
import json
import time
from datetime import datetime, timezone

CONNECTION_STRING_IOT = "HostName=wirtualnygaraz.azure-devices.net;DeviceId=simulatorgarage;SharedAccessKey=mzODsK0gtsXQLnNfb+FNN0/2XC8gwROdmEmahp50NB8="
CONNECTION_STRING_TABLE = "DefaultEndpointsProtocol=https;AccountName=wirtualnygarazbaza;AccountKey=PxJFA9FRjibAli5Blp1KqcO3iaK85qUkEU/6Y697lpJgDQY5zQE4tOxoPz+96rKGZ0EF5wwoANpM+AStmKJHYw==;EndpointSuffix=core.windows.net"

TABLE_NAME = "parkingdata"

table_service = TableServiceClient.from_connection_string(conn_str=CONNECTION_STRING_TABLE)
iot_client = IoTHubDeviceClient.create_from_connection_string(CONNECTION_STRING_IOT)

def create_sample_entity():
    now_iso = datetime.now(timezone.utc).isoformat()
    return {
        "PartitionKey": "garage1",
        "RowKey": now_iso,
        "owner": "Jan Kowalski",        
        "spot_occupied": True,         
        "gate_open": False,             
        "CarType": "sedan",
        "CarPlate": "WE1234A",
        "EntryTime": now_iso,
        "ExitTime": None
    }

def insert_entity(entity):
    try:
        table_client = table_service.get_table_client(TABLE_NAME)
        table_client.create_table()  # jeśli tabela istnieje, nic się nie stanie
    except Exception:
        pass

    table_client = table_service.get_table_client(TABLE_NAME)
    table_client.upsert_entity(mode=UpdateMode.MERGE, entity=entity)
    print("Dane zapisane do tabeli:", entity)

def read_entities():
    table_client = table_service.get_table_client(TABLE_NAME)
    entities = table_client.list_entities()
    return list(entities)

def send_to_iot_hub(entity):
    msg = Message(json.dumps({
        "parking_spot_id": entity["RowKey"],
        "owner": entity["owner"],
        "spot_occupied": entity["spot_occupied"],
        "gate_open": entity["gate_open"],
        "car_type": entity["CarType"],
        "car_plate": entity["CarPlate"],
        "entry_time": entity["EntryTime"],
        "exit_time": entity.get("ExitTime", None)
    }))
    iot_client.send_message(msg)
    print("Wysłano wiadomość do IoT Hub:", msg)

def main():
    iot_client.connect()
    print("Symulator uruchomiony")

    try:
        while True:
            entity = create_sample_entity()
            insert_entity(entity)

            entities = read_entities()
            print("Odczyt danych z tabeli:")
            for e in entities:
                print(e)

            if entities:
                send_to_iot_hub(entities[-1])

            time.sleep(10)
    except KeyboardInterrupt:
        print("Symulator zatrzymany")
    finally:
        iot_client.disconnect()

if __name__ == "__main__":
    main()
