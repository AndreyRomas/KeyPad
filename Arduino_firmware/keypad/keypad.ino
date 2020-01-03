#include <ArduinoJson.h>
#include <Adafruit_GFX.h>
#include <Adafruit_PCD8544.h>
#include <Adafruit_NeoPixel.h>
#include "GyverTimer.h"

#define pwmpin 9
#define ws2812pin 12
#define NUMPIXELS 6

Adafruit_PCD8544 display = Adafruit_PCD8544(3, 4, 5, 6, 7);
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUMPIXELS, ws2812pin, NEO_GRB + NEO_KHZ800);

GTimer_ms myTimer;               // создать таймер

bool intro = true;
byte bl = 255;
String inputString = "";  
String outputString = "";  
bool stringComplete = false;
int data[7] = {0,0,0,0,0,0,0};
bool oldoutput[6] = {0,0,0,0,0,0};
bool output[6] = {0,0,0,0,0,0};
bool outputchanged = false;
int lightmode = 4;

byte ramdomcolor[3] = {0,0,0};

const unsigned char razvodlo [] PROGMEM = 
{0x00, 0x00, 0x7f, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0xff, 0xc0, 0x00, 
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xff, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
0x00, 0x00, 0x00, 0x3f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x0f, 0xf0, 
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x87, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x00, 
0x00, 0x00, 0x00, 0x03, 0xef, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0xee, 
0xf8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xfc, 0x3c, 0x00, 0x00, 0x00, 0x00, 
0x00, 0x00, 0x00, 0x00, 0x01, 0xdd, 0x8c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
0xdd, 0xde, 0x07, 0xff, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7b, 0xde, 0x1f, 0xff, 0xf0, 
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3b, 0xdf, 0x7f, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x00, 
0x00, 0x33, 0xbf, 0x38, 0x00, 0x3e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x79, 0xbf, 0x80, 0x00, 
0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x7f, 0xc0, 0x00, 0x07, 0x80, 0x00, 0x00, 0x00, 
0x00, 0x00, 0xff, 0xff, 0xe0, 0x00, 0x03, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xf0, 
0xe0, 0x01, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xfd, 0xf0, 0x00, 0xe0, 0x00, 0x00, 
0x00, 0x00, 0x00, 0x7c, 0x0f, 0xfb, 0xe8, 0x00, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 
0xf3, 0xdc, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0xe4, 0xfe, 0x00, 0x30, 0x00, 
0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x4f, 0x7f, 0x80, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
0x38, 0x1f, 0xdd, 0xc0, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x3f, 0xe3, 0xe0, 0x38, 
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0x7f, 0xe7, 0xd8, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 
0x00, 0x30, 0xfd, 0x87, 0xbc, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0xf8, 0x0f, 0x7c, 
0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x30, 0xfe, 0x1e, 0xf8, 0x18, 0x00, 0x00, 0x00, 0x00, 
0x00, 0x00, 0x30, 0xff, 0xc5, 0xf0, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x7f, 0xf9, 
0xee, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x3f, 0xfd, 0xbf, 0x98, 0x00, 0x00, 0x00, 
0x00, 0x00, 0x00, 0x18, 0x3f, 0xfe, 0x7f, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1c, 0x1f, 
0xff, 0x7f, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1c, 0x0f, 0xff, 0xff, 0xf8, 0x00, 0x00, 
0x00, 0x00, 0x00, 0x00, 0x0e, 0x0f, 0xff, 0xbf, 0xfe, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 
0x07, 0xff, 0xdf, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x03, 0xff, 0xcf, 0xff, 0xc0, 
0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x83, 0xff, 0xc7, 0xf9, 0xe0, 0x00, 0x00, 0x00, 0x00, 0x00, 
0x01, 0xe3, 0xff, 0x89, 0xf1, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf3, 0xff, 0x9c, 0xf9, 
0x9c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7f, 0xff, 0xfc, 0x7f, 0x0e, 0x00, 0x00, 0x00, 0x00, 
0x00, 0x00, 0x1f, 0xff, 0xf8, 0x1f, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0xc0, 
0x0f, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0xff, 0x80, 0x07, 0x9c, 0x00, 0x00, 0x00, 
0x00, 0x00, 0x00, 0x07, 0xff, 0x80, 0x01, 0xf8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 
0x80, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0xff, 0x80, 0x00, 0x20, 0x00, 0x00};

void setup() 
{
  Serial.begin(9600);
  delay(500);
  
  pixels.begin();
  myTimer.setInterval(500);
  pinMode(pwmpin, OUTPUT);
  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(A2, INPUT);
  pinMode(A3, INPUT);
  pinMode(A4, INPUT);
  pinMode(A5, INPUT);
  analogWrite(pwmpin,bl);
  display.begin();
  display.setContrast(57);
  display.clearDisplay();
  display.display();
  if(!Serial.available()) 
  {
  display.drawBitmap(0, 0,  razvodlo, 84, 48, BLACK);
  display.display();
  delay(3000);
  display.clearDisplay();
  display.println("Romas \nSolutions");
  display.display();
  delay(2000);
  display.clearDisplay();
  display.println("Waiting for PC");
  display.display();
  delay(100);
  }
}

void loop() 
{
  communication();
}

void communication()
{
  serialEvent();
  if(stringComplete == true)
  {
    display.clearDisplay();
    display.setTextSize(1);
    display.setTextColor(BLACK);
    if(inputString.indexOf("#STAR") > -1)
    {
      delay(100);
      Serial.write("#HEY\n");
      display.println("connected");
      display.display();
      delay(500);
      bool intro = false;
    }
    else if(inputString.indexOf("#STOP") > -1)
    {
      display.println("disconnected");
      display.display();
    }
    else if(inputString.indexOf("#ligh") > -1)
    {
      inputString = inputString.substring(6);
      lightmode = inputString.toInt();
    }
    else if(inputString.indexOf("#disp") > -1)
    {
      inputString = inputString.substring(6);
      bl = inputString.toInt() * 25;
      analogWrite(pwmpin,bl);
    }
    else if(inputString.indexOf("#data") > -1)
    {
      inputString = inputString.substring(6);
      byte i=0;
      while(i<=6)
      {
        data[i] = inputString.substring(0,inputString.indexOf("#")).toInt();
        inputString = inputString.substring(inputString.indexOf("#")+1);
        i++;
      }
      display.setCursor(0,0);
      String s1 = "CPU:"+String(data[0])+"C "+String(data[1])+"%";
      String s2 = "GPU:"+String(data[5])+"C "+String(data[6])+"%";
      String s3 = "RAM:"+String(data[2])+"%";
      String s4 = String(data[3])+"/"+String(data[4])+"MB";
      display.println(s1);
      display.println(s2);
      display.println(s3);
      display.println(s4);
      display.display();
    }
    inputString = "";
    stringComplete = false;
}
  
  if(digitalRead(A0) == HIGH) output[0] = true; else output[0] = false;
  if(digitalRead(A1) == HIGH) output[1] = true; else output[1] = false;
  if(digitalRead(A2) == HIGH) output[2] = true; else output[2] = false;
  if(digitalRead(A3) == HIGH) output[3] = true; else output[3] = false;
  if(digitalRead(A4) == HIGH) output[4] = true; else output[4] = false;
  if(digitalRead(A5) == HIGH) output[5] = true; else output[5] = false;
  for (int i1 = 0; i1<6; i1++)
  {
    if (output[i1] == !oldoutput[i1])
    {
      outputchanged = true;
    }
    
  }
  if(outputchanged == true)
  {
    outputString = "b"+String(output[2])+String(output[1])+String(output[0])+String(output[5])+String(output[4])+String(output[3])+"\n";
    for (int i1 = 0; i1 < outputString.length(); i1++)
    {
      Serial.write(outputString[i1]);   // Push each char 1 by 1 on each loop pass
    }
    for (int i1 = 0; i1<6+1; i1=i1+1)
    {
      oldoutput[i1]=output[i1];
    }
    outputString = "";
  }
  outputchanged = false;
  dolight();
  if(lightmode == 3)
  {
    if (myTimer.isReady()) 
    {
      ramdomcolor[0] = random(150);
      ramdomcolor[1] = random(150);
      ramdomcolor[2] = random(150);
    }
  }
}

void serialEvent()
{
    while (Serial.available()) 
    {
      char inChar = (char)Serial.read();
      inputString += inChar;
      if (inChar == '\n')stringComplete = true;
    }
}
void dolight()
{
  bool lightoutput[6] = {0,0,0,0,0,0};
  if(digitalRead(A0) == HIGH) lightoutput[0] = true; else lightoutput[0] = false;
  if(digitalRead(A1) == HIGH) lightoutput[1] = true; else lightoutput[1] = false;
  if(digitalRead(A2) == HIGH) lightoutput[2] = true; else lightoutput[2] = false;
  if(digitalRead(A3) == HIGH) lightoutput[3] = true; else lightoutput[3] = false;
  if(digitalRead(A4) == HIGH) lightoutput[4] = true; else lightoutput[4] = false;
  if(digitalRead(A5) == HIGH) lightoutput[5] = true; else lightoutput[5] = false;
  if(lightmode == 0)
  {  
    for(int i=0;i<NUMPIXELS;i++)
    {
      pixels.setPixelColor(i, pixels.Color(10,10,10));
    }
    if(lightoutput[0] == true) 
    {
    pixels.setPixelColor(0, pixels.Color(100,100,100));
    }
    if(lightoutput[1] == true)
    {
      pixels.setPixelColor(1, pixels.Color(100,100,100));
    }
    if(lightoutput[2] == true) 
    {
      pixels.setPixelColor(2, pixels.Color(100,100,100));
    }
    if(lightoutput[3] == true) 
    {
      pixels.setPixelColor(3, pixels.Color(100,100,100));
    }
    if(lightoutput[4] == true) 
    {
      pixels.setPixelColor(4, pixels.Color(100,100,100));
    }
    if(lightoutput[5] == true) 
    {
      pixels.setPixelColor(5, pixels.Color(100,100,100));
    }
  }
      if(lightmode == 1)
  {  
    for(int i=0;i<NUMPIXELS;i++)
    {
      pixels.setPixelColor(i, pixels.Color(0,0,0));
    }
    if(lightoutput[0] == true) 
    {
    pixels.setPixelColor(0, pixels.Color(100,100,100));
    }
    if(lightoutput[1] == true)
    {
      pixels.setPixelColor(1, pixels.Color(100,100,100));
    }
    if(lightoutput[2] == true) 
    {
      pixels.setPixelColor(2, pixels.Color(100,100,100));
    }
    if(lightoutput[3] == true) 
    {
      pixels.setPixelColor(3, pixels.Color(100,100,100));
    }
    if(lightoutput[4] == true) 
    {
      pixels.setPixelColor(4, pixels.Color(100,100,100));
    }
    if(lightoutput[5] == true) 
    {
      pixels.setPixelColor(5, pixels.Color(100,100,100));
    }
  }
  if(lightmode == 2)
  {  
    for(int i=0;i<NUMPIXELS-3;i++)
    {
      pixels.setPixelColor(i, pixels.Color(0,0,100));
    }
    for(int i=3;i<NUMPIXELS;i++)
    {
      pixels.setPixelColor(i, pixels.Color(100,100,0));
    }
    if(lightoutput[0] == true) 
    {
    pixels.setPixelColor(0, pixels.Color(0,0,200));
    }
    if(lightoutput[1] == true)
    {
      pixels.setPixelColor(1, pixels.Color(0,0,200));
    }
    if(lightoutput[2] == true) 
    {
      pixels.setPixelColor(2, pixels.Color(0,0,200));
    }
    if(lightoutput[3] == true) 
    {
      pixels.setPixelColor(3, pixels.Color(200,200,0));
    }
    if(lightoutput[4] == true) 
    {
      pixels.setPixelColor(4, pixels.Color(200,200,0));
    }
    if(lightoutput[5] == true) 
    {
      pixels.setPixelColor(5, pixels.Color(200,200,0));
    }
  }
  if(lightmode == 3)
  {  
    for(int i=0;i<NUMPIXELS;i++)
    {
      pixels.setPixelColor(i, pixels.Color(20,20,20));
    }
    if(lightoutput[0] == true) 
    {
    pixels.setPixelColor(0, pixels.Color(ramdomcolor[0],ramdomcolor[1],ramdomcolor[2]));
    }
    if(lightoutput[1] == true)
    {
      pixels.setPixelColor(1, pixels.Color(ramdomcolor[0],ramdomcolor[1],ramdomcolor[2]));
    }
    if(lightoutput[2] == true) 
    {
      pixels.setPixelColor(2, pixels.Color(ramdomcolor[0],ramdomcolor[1],ramdomcolor[2]));
    }
    if(lightoutput[3] == true) 
    {
      pixels.setPixelColor(3, pixels.Color(ramdomcolor[0],ramdomcolor[1],ramdomcolor[2]));
    }
    if(lightoutput[4] == true) 
    {
      pixels.setPixelColor(4, pixels.Color(ramdomcolor[0],ramdomcolor[1],ramdomcolor[2]));
    }
    if(lightoutput[5] == true) 
    {
      pixels.setPixelColor(5, pixels.Color(ramdomcolor[0],ramdomcolor[1],ramdomcolor[2]));
    }
  }
  if(lightmode == 4)
  {  
    pixels.setPixelColor(2, pixels.Color(100,0,0));
    pixels.setPixelColor(5, pixels.Color(100,0,0));
    pixels.setPixelColor(1, pixels.Color(0,100,0));
    pixels.setPixelColor(4, pixels.Color(0,100,0));
    pixels.setPixelColor(0, pixels.Color(0,0,100));
    pixels.setPixelColor(3, pixels.Color(0,0,100));
    if(lightoutput[0] == true) 
    {
    pixels.setPixelColor(0, pixels.Color(100,100,100));
    }
    if(lightoutput[1] == true)
    {
      pixels.setPixelColor(1, pixels.Color(100,100,100));
    }
    if(lightoutput[2] == true) 
    {
      pixels.setPixelColor(2, pixels.Color(100,100,100));
    }
    if(lightoutput[3] == true) 
    {
      pixels.setPixelColor(3, pixels.Color(100,100,100));
    }
    if(lightoutput[4] == true) 
    {
      pixels.setPixelColor(4, pixels.Color(100,100,100));
    }
    if(lightoutput[5] == true) 
    {
      pixels.setPixelColor(5, pixels.Color(100,100,100));
    }
  }

  pixels.show(); 
}
