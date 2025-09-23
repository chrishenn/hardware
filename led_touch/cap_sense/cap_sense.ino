#include "TouchSlider.h"




const int SENS_PINS[] = {0};
const int NUM_SENS = sizeof(SENS_PINS) / sizeof(int);

const int LED_PIN = 6;
const int GEN_PIN = 3;


    // TODO: Multiple sensors firings contribute to a smooth interpolated velocity (with acceleration curve?)
    // TODO: figure out how to probe the PWM signal sent to LEDS: should be between 24V and 14V ???

    // TODO: decide on some on/off hardware switch or logic (double tap detection?)


TouchSlider tslider(NUM_SENS, SENS_PINS, LED_PIN);



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

    // OCR2A = 128;
    // OCR2B = 0;

}

void setup() {

    pinMode(LED_PIN, OUTPUT);
    analogWrite(LED_PIN, 255);

    gen_setup();

    Serial.begin(9600);
    // Serial.begin(115200);



    // Serial.println("Calibrating...");
    // tslider.calibrate();
    // Serial.println("Calibration Complete\n\n");

}



void loop()
{
    // tslider.slider();
    tslider.debug();
}
