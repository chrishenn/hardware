/*
Copyright (c) 2010 Aidan Thornton
Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

#include <util/delay_basic.h>

// Pins used. DO NOT MODIFY - some of these are hardcoded elsewhere
// in the code. In particular, the clock and reset pins can't
// easily be changed.
#define LED 8
// pin 9 used for clock
#define DATA 12
#define RESET 10


//#define RAW_DUMP
//#define DOUBLE_RATE // clock card at 8MHz instead of 4MHz
//#define IN_WIRE_ORDER

#define SEND_BITCNT 11
uint8_t send_bits[SEND_BITCNT];
volatile uint8_t scnt, sending;

volatile uint8_t recv, rcnt;

// TIMER2 comp A match - used to send bits on interface
ISR(TIMER2_COMPA_vect) {
  if (++scnt >= SEND_BITCNT) {
      TCCR2B = 0; // disable timer
      TIFR2 = _BV(OCF2A) | _BV(OCF2B); // clear interrupt flags
      digitalWrite(DATA, HIGH);
      pinMode(DATA, INPUT);
      PCIFR = _BV(PCIF0); // clear pin change flag
      PCICR = _BV(PCIE0); // re-enable pin change interrupt

      sending = 0;
  } else {
      digitalWrite(DATA, send_bits[scnt]);
  }
}

// TIMER2 comp B match - used to receive bits on interface.
// The reason we use different comparator registers when sending
// and receiving is because we want to sample the bit in the
// middle of the bit period.
ISR(TIMER2_COMPB_vect) {
  unsigned char rbit = digitalRead(DATA);
#ifdef RAW_DUMP
  if(--rcnt == 0) {
    Serial.write(recv); rcnt = 8;
  }
  recv = (recv << 1) | rbit;
#else

#ifdef IN_WIRE_ORDER
  // dead code, sends out data in the wrong order
  if(--rcnt == 0) {
    TCCR2B = 0; // disable timer
    //PCIFR = _BV(PCIF0); // clear pin change flag
    PCICR = _BV(PCIE0); // re-enable pin change interrupt
    //Serial.write('\n');
    Serial.write('R');
    Serial.write(recv);
  } else {
    //if(rbit == 0) Serial.write('0');
    //if(rbit == 1)  Serial.write('1');
    recv = (recv << 1) | rbit;
  }
#else
  if(rcnt++ == 8) {
    // FIXME - we should check parity here but don't!
    TCCR2B = 0; // disable timer
    //PCIFR = _BV(PCIF0); // clear pin change flag
    PCICR = _BV(PCIE0); // re-enable pin change interrupt
    // report the byte of data to the computer
    Serial.write('.');
    Serial.write(recv);
  } else {
    //if(rbit == 0) Serial.write('0');
    //if(rbit == 1)  Serial.write('1');
    recv |= (rbit << rcnt);
  }

#endif
#endif
}

// We use PCINT0 to detect the start of an incoming byte from
// the smart card and set up timer 2 to sample the incoming bits
ISR(PCINT0_vect) {
  if(digitalRead(DATA) == 0) {
    TCNT2 = 0; //reset timer
    TCCR2B = 0<<CS22 | 1<<CS21 | 0<<CS20;  // enable in divide-by-8 mode
    TIMSK2 = bit(OCIE2B); // turn on compare B interrupt.
    PCICR = 0; // disable pin change interrupt.
 #ifdef IN_WIRE_ORDER
    rcnt = 10;
  #else
    rcnt = 0xfe; recv = 0;
  #endif
    //Serial.write('S');
  }
}

void setup() {
  Serial.begin(115200);
  pinMode(LED, OUTPUT);
  digitalWrite(LED, HIGH);
  delay(100);
  //spiInit();
  digitalWrite(LED, LOW);

  // timer setup

  // TOP=OCR2A, disable
  TCCR2A = _BV(WGM21);
  TCCR2B = 0; // disable
  TIFR2 = _BV(OCF2A) | _BV(OCF2B); // clear interrupt flags

#ifdef DOUBLE_RATE
  OCR2A = 93; // (372/2)/2
  OCR2B = 46; // OCR2A/2
#else
  OCR2A = 186; // 372/2
  OCR2B = 93; // OCR2A/2
#endif

  // TIMSK2 = _BIT(OCIE2A) for compare A interrupt, _BIT(OCIE2B) for compare B

  pinMode(DATA, INPUT);
  digitalWrite(DATA, HIGH); // enable internal pull-up.

   pinMode(RESET, OUTPUT);
   digitalWrite(RESET, LOW);
   delay(50);
   digitalWrite(RESET, HIGH); // FIXME

  // use timer 1 to generate a clock source
  pinMode(9, OUTPUT);
  TCCR1B = 0x00; // disable timer 1
  TCCR1A = 0x40; // normal mode, toggle OC1A on overflow
#ifdef DOUBLE_RATE
  OCR1A = 0; // count to 0 for a clock of 16 MHz / (2*(0+1)) = 8Mhz
#else
  OCR1A = 1; // count to 1 for a clock of 16 MHz / (2*(1+1)) = 4Mhz
#endif
  TCCR1B = 0x09; // reset timer on OCR1 match, prescaler 1, enable.

#ifdef RAW_DUMP
    PCICR = 0; // disable pin change interrupt.
    TCNT2 = 0; //reset timer
    TCCR2B = 0<<CS22 | 1<<CS21 | 0<<CS20;  // enable in divide-by-8 mode
    TIMSK2 = _BV(OCIE2B); // turn on compare B interrupt.
    rcnt = 8;
#else
// Now enable pin change interrupt
  PCMSK0 = _BV(PCINT4);
  PCICR = _BV(PCIE0);
#endif

}

void sendISOChar(uint8_t c) {
    char parity = 0;
  cli();
  TCCR2B = 0; // disable timer
  TIFR2 = _BV(OCF2A) | _BV(OCF2B); // clear interrupt flags
  PCIFR = _BV(PCIF0); // clear pin change flag
  PCICR = 0; // disable pin change interrupt
  sei();



    for(char i = 0; i < 8; i++) {
      parity ^= (send_bits[i+1] = (c >> i) & 1);
    }
    send_bits[9] = parity;
    for(char i = 10; i < SEND_BITCNT; i++) send_bits[i] = 1;

    sending = 1; scnt = 0;
    pinMode(DATA, OUTPUT);
    TCNT2 = 0; //reset timer
    TIMSK2 = bit(OCIE2A); // turn on compare A interrupt.
    TCCR2B = 0<<CS22 | 1<<CS21 | 0<<CS20;  // enable in divide-by-8 mode
    digitalWrite(DATA, LOW);
    while(sending) /* wait */;

#if 1
// Now enable pin change interrupt
  PCMSK0 = _BV(PCINT4);
  PCICR = _BV(PCIE0);
#endif
}

byte readSerial() {
  while (!Serial.available()) {};
  return Serial.read();
}

void loop() {
  char ch;
  ch = readSerial();
  //Serial.print(ch);
  //Serial.print(' ');
  if(ch == 'R') {
   // pulse RESET signal LOW
   digitalWrite(RESET, LOW);
   delay(50);
   digitalWrite(RESET, HIGH);
   Serial.write('A');
  } else if(ch == 'r') {
   // pulse RESET signal HIGH
   digitalWrite(RESET, HIGH);
   delay(50);
   digitalWrite(RESET, LOW);
   Serial.write('A');
  } else if(ch == '.') {
    // send a character in T=0 mode
    char dat = readSerial();
    sendISOChar(dat);
    Serial.write('A');
  } else if(ch == 'T') {
    // send trap byte to card (deep magic)
      uint8_t tc = readSerial();
      uint16_t trap_delay = ((uint16_t)tc << 8) | (uint8_t)readSerial();
      tc = readSerial();
     digitalWrite(RESET, LOW);
     delay(50);
     digitalWrite(RESET, HIGH);
     _delay_loop_2(trap_delay);
     sendISOChar(tc);
     Serial.write('A');
  }
}

