const int buttonPin = 13;
const int ledPin = 12;

bool occupied = false;
unsigned long pressStartTime = 0;

void setup() {
  Serial.begin(115200);
  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  Serial.println("Parking sensor started");
}

void loop() {
  bool buttonState = digitalRead(buttonPin) == LOW;

  if (buttonState && !occupied) {
    occupied = true;
    pressStartTime = millis();
    digitalWrite(ledPin, HIGH);
    Serial.println("wjechano na miejsce parkingowe");
  }
  else if (!buttonState && occupied) {
    occupied = false;
    unsigned long pressDuration = millis() - pressStartTime;
    digitalWrite(ledPin, LOW);
    Serial.print("opuszczono miejsce parkingowe - czas trwania: ");
    Serial.print(pressDuration / 1000.0); 
    Serial.println(" s");
    pressStartTime = 0; 
  }

  delay(50);
}