from azure.data.tables import TableServiceClient, UpdateMode
from azure.iot.device import IoTHubDeviceClient, Message
import json
import time
import threading
from datetime import datetime, timezone

CONNECTION_STRING_IOT = "HostName=wirtualnygaraz.azure-devices.net;DeviceId=simulatorgarage;SharedAccessKey=mzODsK0gtsXQLnNfb+FNN0/2XC8gwROdmEmahp50NB8=" 
CONNECTION_STRING_TABLE = "DefaultEndpointsProtocol=https;AccountName=wirtualnygaraz1234;AccountKey=ce9B9Bf9opzPWkBe9QtUiPivQbq9p+g223ZdUKFaxwxv7GH4TQxxDmZVuca4s1G9tKIqixLyWF4++ASttDO2Zg==;EndpointSuffix=core.windows.net"

TABLE_NAME = "parkingdata"

table_service = TableServiceClient.from_connection_string(conn_str=CONNECTION_STRING_TABLE)
iot_client = IoTHubDeviceClient.create_from_connection_string(CONNECTION_STRING_IOT)

try:
    tables = table_service.list_tables()
    print("✅ Połączono z Table Storage! Lista tabel:")
    for table in tables:
        print("-", table.name)
except Exception as e:
    print("❌ Błąd połączenia z Table Storage:", e)

def insert_or_update_entity(parking_spot, status, gate_status, timestamp_iso):
    table_client = table_service.get_table_client(table_name=TABLE_NAME)
    entity = {
        "PartitionKey": parking_spot,
        "RowKey": "current",
        "IsCar": status,
        "Status": gate_status,
        "TimestampISO": timestamp_iso
    }
    try:
        table_client.upsert_entity(mode=UpdateMode.REPLACE, entity=entity)
        print("✅ Zaktualizowano dane w Table Storage")
    except Exception as e:
        print("❌ Błąd aktualizacji danych w Table Storage:", e)

# Flagi stanu
stop_requested = False
gate_open = False  # flaga stanu bramy

def input_listener():
    global stop_requested, gate_open
    while True:
        user_input = input().strip().lower()
        if user_input == "stop" and not stop_requested:
            stop_requested = True
            print("\n🚗 Pojazd wyjechał z miejsca parkingowego.")
        elif user_input == "open":
            gate_open = True
            print("\n🟢 Brama została otwarta.")
        elif user_input == "close":
            gate_open = False
            print("\n🔴 Brama została zamknięta.")

def main():
    global stop_requested, gate_open

    print("Symulator czujnika parkingowego uruchomiony")
    answer = input("Czy pojazd wjechał na miejsce parkingowe? (t/n): ").strip().lower()

    if answer != 't':
        print("Brak pojazdu na miejscu. Symulacja zakończona.")
        return

    garage_name = "garage1"

    listener_thread = threading.Thread(target=input_listener, daemon=True)
    listener_thread.start()

    try:
        while True:
            status = True if not stop_requested else False
            gate_status = True if gate_open else False
            timestamp_iso = datetime.now(timezone.utc).isoformat()

            print("\n--- Aktualizacja danych ---")
            print(f"Miejsce parkingowe: {garage_name}")
            print(f"Czy miejsce jest zajęte: {status}")
            print(f"Brama: {gate_status}")

            insert_or_update_entity(
                parking_spot=garage_name,
                status=gate_open,
                gate_status=gate_status,
                timestamp_iso=timestamp_iso
            )

            time.sleep(10)

    except KeyboardInterrupt:
        print("\nSymulator zatrzymany")

if __name__ == "__main__":
    main()
