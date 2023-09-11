int x = 0;

void setup() {
  Serial.begin(9600);  
}

void loop() {
  Serial.print("loop: ");
  Serial.println(x);
  x++;
  delay(2000); // 2 segundos
}