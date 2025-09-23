const int NUM_SENS = 4;

const int THRESH_SENS = 38;

const int SENSE_1 = 2;
const int SENSE_2 = 4;
const int SENSE_3 = 6;
const int SENSE_4 = 8;

const int OUT_1 = 3;
const int OUT_2 = 5;
const int OUT_3 = 7;
const int OUT_4 = 9;

const int sens_pins[] = {SENSE_1, SENSE_2, SENSE_3, SENSE_4};
const int out_pins[] = {OUT_1, OUT_2, OUT_3, OUT_4};



  // TODO: add calibration period, to listen for max values on each sensor
  // TODO: support clibration array, with different THRESH_SENS for each sensor
  // TODO: logic to detect a swipe and adjust PWM output
  // TODO: add persistent PWM output
  // TODO: decide on some on/off logic (double tap detection?)
  // TODO: the PWM signal is inverted by the electronic circuit: need to invert logically in the math
  


void setup() {

  for (int i = 0; i < NUM_SENS; i++){
    pinMode(out_pins[i], OUTPUT);
    pinMode(sens_pins[i], INPUT);    
  }

  Serial.begin(9600);

}


void loop() {

  long sensVal;
  int i = 0;


  // travels through sensor indexes from 0 to num_sens-1 once and exits, unless a touch is detected
  while (true)
  {    
    sensVal = read_sens( out_pins[i], sens_pins[i] );

//    if (i == 0) write_found(i, sensVal);  // debug


    if (sensVal > THRESH_SENS)
    {
      write_found(i, sensVal);


      // check sensors on either side for some loops
      int left_i = i - 1;
      int right_i = i + 1;
      
      for (int j = 0; j < 2; j++)
      {
        
        if (left_i >= 0)
        {
          sensVal = read_sens( out_pins[left_i], sens_pins[left_i] );
          if (sensVal > THRESH_SENS){
            write_found(left_i, sensVal);
            i = left_i;
            break;
          }                       
        }
        
        if (right_i < NUM_SENS)
        {
          sensVal = read_sens( out_pins[right_i], sens_pins[right_i] );
          if (sensVal > THRESH_SENS){
            write_found(right_i, sensVal);
            i = right_i;
            break;
          }          
        }
    
      }
           
    }
    else 
    {
      i++;
      if (i >= NUM_SENS) break;      
    }
    

  }
     
}
    



void write_found(int pin_num, long sensVal){
    Serial.write( "sensor " );
    Serial.print(pin_num);
    Serial.write( ":   " );
    Serial.print(sensVal);
    Serial.println();   
}

long read_sens( int OUT_PIN, int SENS_PIN ){

  digitalWrite(OUT_PIN, LOW);
  delay(10);

  long startTime = micros();

  digitalWrite(OUT_PIN, HIGH);
  
  while( digitalRead(SENS_PIN) == LOW );
  
  long sensVal = micros() - startTime;

  return sensVal;

}

long read_sens_av( int OUT_PIN, int SENS_PIN, int num_samples ){

  long sensVal_sum = 0;

  for (int i = 0; i < num_samples; i++){

    digitalWrite(OUT_PIN, LOW);
    delay(10);
  
    long startTime = micros();
  
    digitalWrite(OUT_PIN, HIGH);
    
    while( digitalRead(SENS_PIN) == LOW );
    
    sensVal_sum += micros() - startTime;     
  }

  return sensVal_sum / num_samples;

}
