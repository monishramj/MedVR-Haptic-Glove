#include <ESP32Servo.h>

// Create the five servos
Servo thumbservo;
Servo pointservo;
Servo middleservo;
Servo ringservo;
Servo pinkyservo;

// Pin storage for servos
int thumbservoPin = 11;
int pointservoPin = 10;
int middleservoPin = 9;
int ringservoPin = 8;
int pinkyservoPin = 7;

// Pin storage for potentiometers
int thumbpotPin = A0;
int pointpotPin = A1;
int middlepotPin = A2;
int ringpotPin = A3;
int pinkypotPin = A4;

// Value storages
int thumbval, pointval, middleval, ringval, pinkyval;
int thumbcurl, pointcurl, middlecurl, ringcurl, pinkycurl;
int offset = 10;

// Previous values for change detection
int prevThumbcurl = -1;
int prevPointcurl = -1;
int prevMiddlecurl = -1;
int prevRingcurl = -1;
int prevPinkycurl = -1;

int ADC_Max = 4096;

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

  thumbservo.write(0);
  pointservo.write(0);
  middleservo.write(0);
  ringservo.write(180);
  pinkyservo.write(180);
}

void loop()
{
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

  // Only send if any values changed
  /*if (thumbcurl != prevThumbcurl ||
      pointcurl != prevPointcurl ||
      middlecurl != prevMiddlecurl ||
      ringcurl != prevRingcurl ||
      pinkycurl != prevPinkycurl) {*/

    Serial.print("POT:");
    Serial.print(thumbcurl);  Serial.print(",");
    Serial.print(pointcurl);  Serial.print(",");
    Serial.print(middlecurl); Serial.print(",");
    Serial.print(ringcurl);   Serial.print(",");
    Serial.println(pinkycurl);

    /*prevThumbcurl  = thumbcurl;
    prevPointcurl  = pointcurl;
    prevMiddlecurl = middlecurl;
    prevRingcurl   = ringcurl;
    prevPinkycurl  = pinkycurl;*/
  //}

  // Servo mappings
  /*thumbval  = map(thumbval, 0, 3000, 180 - 1.5 * offset, 0 - 1.5 * offset);
  pointval  = map(pointval, 0, 3060, 180 - offset, 0 - offset);
  middleval = map(middleval, 0, 3100, 180 - 5 * offset, 0 - 5 * offset);
  ringval   = map(ringval, 4095, 995, 0 + 3 * offset, 180 + 3 * offset);
  pinkyval  = map(pinkyval, 4095, 780, 0 + 3 * offset, 180 + 3 * offset);

  // Move servos
  thumbservo.write(thumbval);
  pointservo.write(pointval);
  middleservo.write(middleval);
  ringservo.write(ringval);
  pinkyservo.write(pinkyval);*/


  delay(20); 
}
