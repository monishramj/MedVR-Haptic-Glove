#include <ESP32Servo.h>

Servo thumbservo, pointservo, middleservo, ringservo, pinkyservo;

int thumbservoPin = 11;
int pointservoPin = 10;
int middleservoPin = 9;
int ringservoPin = 8;
int pinkyservoPin = 7;

int thumbpotPin = A0;
int pointpotPin = A1;
int middlepotPin = A2;
int ringpotPin = A3;
int pinkypotPin = A4;

int thumbval, pointval, middleval, ringval, pinkyval;
int thumbcurl, pointcurl, middlecurl, ringcurl, pinkycurl;
int offset = 10;

int ADC_Max = 4096;

bool lockThumb = false;
bool lockPointer = false;
bool lockMid = false;
bool lockRing = false;
bool lockPinky = false;

int thumbLockAngle, pointerLockAngle, midLockAngle, ringLockAngle, pinkyLockAngle;

// Default "rest" positions when not engaged
const int DEFAULT_THUMB = 0;
const int DEFAULT_POINT = 0;
const int DEFAULT_MIDDLE = 0;
const int DEFAULT_RING = 180; // Flipped around
const int DEFAULT_PINKY = 180;

void setup()
{
  Serial.begin(115200); 

  ESP32PWM::allocateTimer(0);
  ESP32PWM::allocateTimer(1);
  ESP32PWM::allocateTimer(2);
  ESP32PWM::allocateTimer(3);

  thumbservo.setPeriodHertz(50);
  pointservo.setPeriodHertz(50);
  middleservo.setPeriodHertz(50);
  ringservo.setPeriodHertz(50);
  pinkyservo.setPeriodHertz(50);

  thumbservo.attach(thumbservoPin, 500, 2400);
  pointservo.attach(pointservoPin, 500, 2400);
  middleservo.attach(middleservoPin, 500, 2400);
  ringservo.attach(ringservoPin, 500, 2400);
  pinkyservo.attach(pinkyservoPin, 500, 2400);

  thumbservo.write(DEFAULT_THUMB);
  pointservo.write(DEFAULT_POINT);
  middleservo.write(DEFAULT_MIDDLE);
  ringservo.write(DEFAULT_RING);
  pinkyservo.write(DEFAULT_PINKY);
}

void loop()
{
  checkSerial();
    
  // Read potentiometer values
  thumbval  = analogRead(thumbpotPin);
  pointval  = analogRead(pointpotPin);
  middleval = analogRead(middlepotPin);
  ringval   = analogRead(ringpotPin);
  pinkyval  = analogRead(pinkypotPin);

  // Map potentiometer values to curl range
  thumbcurl  = map(thumbval, 0, 3250, 0, 68);
  pointcurl  = map(pointval, 0, 3250, 0, 68);
  middlecurl = map(middleval, 0, 3350, 0, 68);
  ringcurl   = map(ringval, 4095, 900, 0, 68);
  pinkycurl  = map(pinkyval, 4095, 1000, 0, 68);

  // Clamp negative values
  thumbcurl  = max(0, thumbcurl);
  pointcurl  = max(0, pointcurl);
  middlecurl = max(0, middlecurl);
  ringcurl   = max(0, ringcurl);
  pinkycurl  = max(0, pinkycurl);

    Serial.print("POT:");
    Serial.print(thumbcurl);  Serial.print(",");
    Serial.print(pointcurl);  Serial.print(",");
    Serial.print(middlecurl); Serial.print(",");
    Serial.print(ringcurl);   Serial.print(",");
    Serial.println(pinkycurl);

  // If locked, use last recorded angle from potentiometer
  // If unlocked, use default rest position
  int thumbAngle = lockThumb
                   ? map(thumbLockAngle, 0, 3000, 180 - 1.5 * offset, 0 - 1.5 * offset)
                   : DEFAULT_THUMB;
  int pointAngle = lockPointer
                   ? map(pointerLockAngle, 0, 3060, 180 - offset, 0 - offset)
                   : DEFAULT_POINT;
  int middleAngle = lockMid
                   ? map(midLockAngle, 0, 3100, 180 - 5 * offset, 0 - 5 * offset)
                   : DEFAULT_MIDDLE;
  int ringAngle = lockRing
                   ? map(ringLockAngle, 4095, 995, 0 + 3 * offset, 180 + 3 * offset)
                   : DEFAULT_RING;
  int pinkyAngle = lockPinky
                   ? map(pinkyLockAngle, 4095, 780, 0 + 3 * offset, 180 + 3 * offset)
                   : DEFAULT_PINKY;

  // Write angles to servos
  thumbservo.write(thumbAngle);
  pointservo.write(pointAngle);
  middleservo.write(middleAngle);
  ringservo.write(ringAngle);
  pinkyservo.write(pinkyAngle);

  delay(20); 
}

//Checks Serial to update Servos based on Unity write
void checkSerial() {

  if (Serial.available()) {
    String input = Serial.readStringUntil('\n');
    input.trim();
    
    if (input.startsWith("SERVO:")) {
      int spaceIndex = input.indexOf(' ');
      if (spaceIndex > 6) {
        String name = input.substring(6, spaceIndex);
        String state = input.substring(spaceIndex + 1);

        bool lock = (state == "1");

        if (name == "Thumb") {
          if (lock && !lockThumb) thumbLockAngle = thumbval;
          lockThumb = lock;
        } else if (name == "Pointer") {
          if (lock && !lockPointer) pointerLockAngle = pointval;
          lockPointer = lock;
        } else if (name == "Mid") {
          if (lock && !lockMid) midLockAngle = middleval;
          lockMid = lock;
        } else if (name == "Ring") {
          if (lock && !lockRing) ringLockAngle = ringval;
          lockRing = lock;
        } else if (name == "Pinky") {
          if (lock && !lockPinky) pinkyLockAngle = pinkyval;
          lockPinky = lock;
        }
      }
    }
  }
}

