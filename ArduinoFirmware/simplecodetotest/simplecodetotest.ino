int angulo1 = 0; // Ângulo no plano XY
int angulo2 = 0; // Ângulo em relação ao eixo Z
int delayTime = 1000; // Tempo de atraso inicial para o envio dos dados

void setup() {
  Serial.begin(9600);
}

void loop() {
  // Enviar as coordenadas esféricas
  int distancia = random(30, 46); // Gera um valor aleatório entre 30 e 45 para a distância
  String jsonString = "{ \"angulo1\": " + String(angulo1) + ", \"angulo2\": " + String(angulo2) + ", \"distancia\": " + String(distancia) + " }";
  Serial.println(jsonString);
  
  // Incrementa os ângulos
  angulo1++;
  if (angulo1 > 90) {
    angulo1 = 0;
    angulo2++;
    if (angulo2 >= 360) {
      angulo2 = 0;
    }
  }

  // Verificar se há dados disponíveis na porta serial
  if (Serial.available()) {
    char receivedChar = Serial.read();

    switch (receivedChar) {
      case 'R':  // Reinicia o Arduino
        asm volatile ("  jmp 0");
        break;
      case 'Z':  // Zera os ângulos
        angulo1 = 0;
        angulo2 = 0;
        break;
      case '+':  // Aumenta a frequência de envio dos dados
        delayTime = max(100, delayTime - 100);
        break;
      case '-':  // Diminui a frequência de envio dos dados
        delayTime = min(10000, delayTime + 100);
        break;
    }
  }
  
  delay(delayTime); // Respeita o tempo de atraso para o envio dos dados
}

