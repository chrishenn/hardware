
const int pwm_pin = 5;
const int temp_pin_0 = A1;
const int temp_pin_1 = A3;

const int N_SAMPLES = 100;
const int N_MILLI = 1000;
const float AREF_VOLTAGE = 3.3;
const int DUTY_MAX = 255;

const int DUTY_PERC_MAX = 100;
const int DUTY_PERC_MIN = 40;

const int CPU_TEMP_OFFSET = 29;
const int GPU_TEMP_OFFSET = 0;

int duty_int = DUTY_MAX;



void setup() {
    Serial.begin(9600);

    pinMode(pwm_pin, OUTPUT);

    analogReference(EXTERNAL);
}


void loop(void) {

    analogWrite(pwm_pin, duty_int);

    // accumulate some number of samples from each thermistor
    float temp_sum_0 = 0;
    float temp_sum_1 = 0;
    for (int i = 0; i < N_SAMPLES; i++){
        temp_sum_0 += float( analogRead(temp_pin_0) );
        delay(N_MILLI / N_SAMPLES / 2);
        temp_sum_1 += float( analogRead(temp_pin_1) );
        delay(N_MILLI / N_SAMPLES / 2);
    }

    // average them
    float av_temp_0 = temp_sum_0 / float( N_SAMPLES );
    float av_temp_1 = temp_sum_1 / float( N_SAMPLES );

    // reported temps from each thermistor, according to its voltage
    float temp_0 = ( ( (av_temp_0 * AREF_VOLTAGE) / 1024.0) - 0.5) * 100.0;
    float temp_1 = ( ( (av_temp_1 * AREF_VOLTAGE) / 1024.0) - 0.5) * 100.0;

    temp_0 = (temp_0 * 1.9665) - 9.0562;        // from regression curve to estimate real temp
    int duty_perc_0 = int( temp_0 * (5.0/4.0) + 12.5 ); // from real temp to duty percent

    temp_1 = (temp_1 * 1.29795) + 36.556;       // from regression curve to estimate real temp
    int duty_perc_1 = int( temp_1 * (5.0/4.0) );    // from real temp to duty percent

    Serial.print("Current temp_0: ");
    Serial.println(temp_0);
    Serial.print("Current temp_1: ");
    Serial.println(temp_1);

    Serial.print("duty_0: ");
    Serial.println(duty_perc_0);
    Serial.print("duty_1: ");
    Serial.println(duty_perc_1);

    // take the biggest recommended duty percent from any thermistor
    int duty_perc = max(duty_perc_0, duty_perc_1);
    duty_perc = max(duty_perc, DUTY_PERC_MIN);
    duty_perc = min(duty_perc, DUTY_PERC_MAX);

    // convert from duty percent to integer out of 255
    duty_int = (float(duty_perc) / 100.0) * DUTY_MAX;

    Serial.print("duty_perc (out of 100): ");
    Serial.println(duty_perc);

    Serial.println();
    Serial.println();
}

