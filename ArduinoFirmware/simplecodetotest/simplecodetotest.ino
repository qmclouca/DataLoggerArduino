int x = 0;
int y = 0;
int z = 0;

void setup() {
  Serial.begin(9600);
}

void loop() {
  String jsonString = "{ \"x\": " + String(x) + ", \"y\": " + String(y) + ", \"z\": " + String(z) + " }";
  Serial.println(jsonString);
  
  x++;
  y++;
  z++;
  delay(2000); // 2 segundos
}
