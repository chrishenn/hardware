

// valid PWM pins for arduino nano: 3, 5, 6, 9, 10, 11  490 Hz (pins 5 and 6: 980 Hz)
// Timer_0: pins 5,6
// Timer_1: pins 9,10
// Timer_2: pins 3, 11

const int pwm_pin = 5;

const int pwm_pins[] = {3, 5, 6, 9, 10, 11};
const int n_pwm_pins = 6;




void gen_setup(){

    // TCCR2A - [COM2A1, COM2A0, COM2B1, COM2B0, reserved, reserved, WGM21, WGM20]
    // TCCR2B - [FOC2A, FOC2B, reserved, reserved, WGM22, CS22, CS21, CS20]

    // set timer 2 (connected to pin 3, or GEN_PIN) with prescalar = 1
    //  and set that timer to continuous 25% duty cycle in fast PWM mode
    //  with start and stop at 0, 3, giving 4 MHz PWM

    pinMode(GEN_PIN, OUTPUT);

    TCCR2A = _BV(COM2A0) | _BV(COM2B1) | _BV(WGM21) | _BV(WGM20);
    TCCR2B = _BV(WGM22) | _BV(CS20);
    OCR2A = 3;
    OCR2B = 0;

}


void setup() {

//  pinMode(pwm_pin, OUTPUT);


  for (int i = 0; i < n_pwm_pins; i++){

    pinMode(pwm_pins[i], OUTPUT);

  }


//  TCCR1B = TCCR1B & B11111000 | B00000001;
  TCCR2B = TCCR2B & B11111000 | B00000001;

  Serial.begin(9600);

}


void loop() {

  int duty_int = 140;

//  for (int duty_int = 0; duty_int < 255; duty_int += 50){

//    analogWrite(pwm_pin, duty_int);
//    Serial.println(duty_int);
//    delay(1000);


  for (int i = 0; i < n_pwm_pins; i++){

    analogWrite(pwm_pins[i], duty_int);

  }

  delay(10000);

//  }


}
