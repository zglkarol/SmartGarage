#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <PubSubClient.h>

// WiFi
const char* ssid = "iPhone11";
const char* password = "Adam2003";

// Azure IoT Hub
const char* mqttServer = "wirtualnygaraz.azure-devices.net";
const int mqttPort = 8883;
const char* deviceId = "simulatorgarage";

// SAS token (upewnij się, że jest aktualny!)
const char* sasToken = "SharedAccessSignature sr=wirtualnygaraz.azure-devices.net%2Fdevices%2Fsimulatorgarage&sig=91PdaY9MlXE79KLqLWQYq9glU40iIHNMWp9WGHy%2BSFY%3D&se=1748042824";

// Pełny certyfikat root CA Azure IoT Hub (TU DAJ PEŁNY!)
// Dla przykładu możesz pobrać z https://www.digicert.com/CACerts/BaltimoreCyberTrustRoot.crt.pem
const char azure_root_ca[] PROGMEM = R"EOF(
-----BEGIN CERTIFICATE-----
MIIDdzCCAl+gAwIBAgIEbLkC+jANBgkqhkiG9w0BAQUFADBmMQswCQYDVQQGEwJV
UzEWMBQGA1UEChMNRGlnaUNlcnQgSW5jMRwwGgYDVQQLExNHaWduYXR1cmUgUm9v
dCBDQTAeFw0xNzAxMDEwMDAwMDBaFw0yNzAxMDEwMDAwMDBaMGYxCzAJBgNVBAYT
AlVTMRgwFgYDVQQKEw9EaWdpQ2VydCBJbmMxHDAaBgNVBAsTE0dpZ25hdHVyZSBS
b290IENBMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwUQlh43u2By+
EjHNCW8JZ/az50UxHOxU6mg2mjTh73RhTYZmNzNx6ObWrcZQeMlhb2J32JoVL02c
...
-----END CERTIFICATE-----
)EOF";

// Piny
const int buttonPin = 13;
const int ledPin = 12;

// Zmienne do logiki przycisku
bool occupied = false;
unsigned long pressStartTime = 0;

// Połączenia
WiFiClientSecure espClient;
PubSubClient client(espClient);

void setup() {
  Serial.begin(115200);

  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);

  Serial.println("Parking sensor started");

  connectWiFi();

  espClient.setCACert(azure_root_ca);

  client.setServer(mqttServer, mqttPort);
}

void loop() {
  if (!client.connected()) {
    connectToIoTHub();
  }
  client.loop();

  bool buttonState = digitalRead(buttonPin) == LOW;

  if (buttonState && !occupied) {
    occupied = true;
    pressStartTime = millis();
    digitalWrite(ledPin, HIGH);
    Serial.println("Wjechano na miejsce parkingowe");
  } 
  else if (!buttonState && occupied) {
    occupied = false;
    unsigned long pressDuration = millis() - pressStartTime;
    digitalWrite(ledPin, LOW);
    Serial.print("Opuszczono miejsce parkingowe - czas trwania: ");
    Serial.print(pressDuration / 1000.0);
    Serial.println(" s");

    // Wysyłamy dane do IoT Hub
    sendStatusToIoTHub(true, pressDuration);  // zajęte i czas
    delay(100);  // krótka przerwa, żeby wysłać kolejny status
    sendStatusToIoTHub(false, 0);              // miejsce wolne, czas 0

    pressStartTime = 0;
  }

  delay(50);
}

void sendStatusToIoTHub(bool isOccupied, unsigned long durationMs) {
  if (!client.connected()) {
    Serial.println("Nie połączono z IoT Hub");
    return;
  }

  String payload = "{";
  payload += "\"occupied\":";
  payload += (isOccupied ? "true" : "false");
  payload += ",\"duration_ms\":";
  payload += durationMs;
  payload += "}";

  String topic = "devices/";
  topic += deviceId;
  topic += "/messages/events/";

  Serial.print("Wysyłam do IoT Hub: ");
  Serial.println(payload);

  client.publish(topic.c_str(), payload.c_str());
}

void connectWiFi() {
  WiFi.begin(ssid, password);
  Serial.print("Łączenie z WiFi ");
  int retry = 0;
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    retry++;
    if (retry > 40) {  // po 20 sekundach timeout
      Serial.println("\nNie udało się połączyć z WiFi");
      return;
    }
  }
  Serial.println("\nPołączono z WiFi");
  Serial.print("IP: ");
  Serial.println(WiFi.localIP());
}

void connectToIoTHub() {
  Serial.print("Łączenie z IoT Hubem... ");

  String username = String(mqttServer) + "/" + deviceId + "/?api-version=2018-06-30";

  if (client.connect(deviceId, username.c_str(), sasToken)) {
    Serial.println("Połączono z IoT Hub");
  } else {
    Serial.print("Błąd połączenia, rc=");
    Serial.print(client.state());
    Serial.println(" - spróbuję ponownie za 5s");
    delay(5000);
  }
}
