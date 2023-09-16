int x = 0;
int y = 0;
int z = 0;
int delayTime = 2000;  // Define o delay inicial como 2 segundos

void setup() {
  Serial.begin(9600);
}

void loop() {
  // Enviar as coordenadas
  String jsonString = "{ \"x\": " + String(x) + ", \"y\": " + String(y) + ", \"z\": " + String(z) + " }";
  Serial.println(jsonString);
  
  // Verificar se há dados disponíveis na porta serial
  if (Serial.available()) {
    char receivedChar = Serial.read();

    switch (receivedChar) {
      case 'R':  // Reinicia o Arduino
        asm volatile ("  jmp 0");
        break;
      case 'Z':  // Zera as coordenadas
        x = 0;
        y = 0;
        z = 0;
        break;
      case '+':  // Aumenta a frequência de envio dos dados
        delayTime = max(500, delayTime - 500);
        break;
      case '-':  // Diminui a frequência de envio dos dados
        delayTime += 500;
        break;
    }
  }
  
  x++;
  y++;
  z++;
  delay(delayTime);
}

