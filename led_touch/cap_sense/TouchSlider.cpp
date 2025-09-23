

#include "TouchSlider.h"

#include "Arduino.h"



const int DUTY_MAX = 255;


void write_all(int pin_num, float value){
    Serial.print( "sensor pin " );
    Serial.print(pin_num);
    Serial.print( ":     " );
    Serial.println(value);
}

void write_val(int value){
    Serial.println(value);
}





TouchSlider::TouchSlider( const int NUM_SENS, const int* SENS_PINS, const int LED_PIN )
{
    this->NUM_SENS = NUM_SENS;
    this->SENS_PINS = SENS_PINS;

    this->LED_PIN = LED_PIN;
    this->LED_STATE = 0; // in percent
    this->LED_STEP = 10; // in percent

    this->noise = new int[NUM_SENS];
    this->thresh = new int[NUM_SENS];
}

TouchSlider::~TouchSlider()
{
    delete[] this->noise;
    delete[] this->thresh;
}




// void TouchSlider::calibrate_old()
// {
//
//     Serial.print("Num active pins: ");
//     Serial.println(this->NUM_SENS);
//
//     Serial.print("Active sensing pins: ");
//     for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++){
//         Serial.print(this->SENS_PINS[sens_i]);
//         Serial.print(", ");
//     }
//     Serial.println();
//
//     const int n_samples = 10000;
//
//     for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++)
//     {
//         int temp_sum = 0;
//         int max = 0;
//         for (int j = 0; j < n_samples; j++)
//         {
//             int tmp = analogRead( this->SENS_PINS[sens_i] );
//             temp_sum += tmp;
//             if (tmp > max) max = tmp;
//         }
//
//         int average = max( int( temp_sum / n_samples ), 0 );
//
//         this->noise[sens_i] = average;
//
//         int thresh = int( float(max - average) * 0.5 );
//         // this->thresh[sens_i] = max( 20, thresh );
//
//         this->thresh[sens_i] = 40;
//
//         Serial.print("Sensor pin: ");
//         Serial.print(this->SENS_PINS[sens_i]);
//         Serial.print(" noise average: ");
//         Serial.print(this->noise[sens_i]);
//         Serial.print(" noise max: ");
//         Serial.print(max);
//         Serial.print(" Trigger Thresh: ");
//         Serial.print(this->thresh[sens_i]);
//         Serial.print("\n");
//     }
//
// }





void TouchSlider::calibrate()
{
    Serial.print("Num active pins: ");
    Serial.println(this->NUM_SENS);

    Serial.print("Active sensing pins: ");
    for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++){
        Serial.print(this->SENS_PINS[sens_i]);
        Serial.print(", ");
    }
    Serial.println();


    const int n_samples = 1000;

    // zero the noise floor
    for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++) this->noise[sens_i] = 4;
    for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++) this->thresh[sens_i] = 4;



    // int run_avs[this->NUM_SENS] = { 0 };
    //
    // const float alpha = 0.2;
    // const float alpha_inv = 1 - alpha;
//
    // for (int brightness = 0; brightness < 256; brightness++)
    // {
    //     analogWrite(this->LED_PIN, brightness);
    //     delay(100);
    //
    //     for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++)
    //     {
    //
    //         int temp_sum = 0;
    //         for (int i = 0; i < n_samples; i++) temp_sum += analogRead( this->SENS_PINS[sens_i] );
    //         float new_val = float(temp_sum) / float(n_samples);
    //
    //         float av_new = alpha * new_val + alpha_inv * run_avs[sens_i];
    //         run_avs[sens_i] = av_new;
    //
    //         Serial.println(av_new);
    //     }
    // }

}










float TouchSlider::sample_pin(const int sens_i, const int n_samples )
{
    int temp_sum = 0;
    for (int j = 0; j < n_samples; j++) temp_sum += analogRead( this->SENS_PINS[sens_i] );

    return float(temp_sum) / float(n_samples);
}

void TouchSlider::set_led_state(int state){
    state = max(state, 0);
    state = min(state, 100);

    this->LED_STATE = state;
}

int TouchSlider::perc_to_led_int(int perc_state) {

    int duty_int = (float(perc_state) / 100.0) * DUTY_MAX;

    duty_int = max(duty_int, 0);
    duty_int = min(duty_int, DUTY_MAX);

    return duty_int;
}

void TouchSlider::draw_state_to_led() {
    analogWrite( this->LED_PIN, this->perc_to_led_int(this->LED_STATE) );
}

int TouchSlider::clamp_index_bounds(int sens_i) {

    sens_i = max(sens_i, 0);
    sens_i = min(sens_i, this->NUM_SENS - 1);

    return sens_i;
}






void TouchSlider::slider()
{
    int sens_i = 0;
    int bot_limit = 0;
    int top_limit = this->NUM_SENS - 1;

    bool flag_up = true;

    int ping_count = 0;
    bool ping_flag = false;
    const int ping_timeout = 100;  // in number of samples

    float momentum = 0;
    int prev_hit = 0;

    float run_avs[this->NUM_SENS] = { 0 };

    const float alpha = 0.05;
    const float alpha_inv = 1 - alpha;

    int g_count = 0;

    bool active[this->NUM_SENS] = { false };
    const float sig_thresh_perc = 0.8;

    float finger_pos = this->NUM_SENS / 2;
    bool finger_flag = false;
    int finger_timer = 0;
    const int timeout_thresh = 100 * this->NUM_SENS;

    float velocity = 0;


    while (true)
    {

        float sig_av = 0;

        for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++) {

            float new_val = analogRead( this->SENS_PINS[sens_i] );

            float av_new = alpha * new_val + alpha_inv * run_avs[sens_i];
            av_new = max(av_new, 0);
            run_avs[sens_i] = av_new;

            // Serial.print(av_new);
            // Serial.print(" ");

            sig_av += av_new;
        }

        sig_av /= this->NUM_SENS;

        // Serial.print(sig_av);
        // Serial.print(" ");
        // Serial.println();

        int n_active = 0;
        for (int sens_i = 0; sens_i < this->NUM_SENS; sens_i++) {

            bool act = (run_avs[sens_i] - sig_av) > (sig_thresh_perc * sig_av);

            // if (act) Serial.print(1);
            // else Serial.print(0);
            // Serial.print(" ");

            if (act) {

                if (!finger_flag) {
                    finger_pos = float(sens_i);
                    finger_flag = true;
                }
                else {
                    float finger_pos_new = alpha * sens_i + alpha_inv * finger_pos;

                    velocity += (finger_pos_new - finger_pos);

                    finger_pos = finger_pos_new;

                    Serial.println(finger_pos);
                    // Serial.print(finger_pos);

                    // Serial.print(" ");
                    // Serial.println(velocity);

                    // if (velocity > 0.1) Serial.println(velocity);
                }

                finger_timer = 0;

            } else finger_timer++;
        }

        // Serial.print(sig_thresh_perc * sig_av);
        // Serial.println();


        if ( finger_flag && (finger_timer > timeout_thresh) ) {
            finger_flag = false;
            velocity = 0;

            Serial.println("timeout");
        }



        // g_count++;
        // int step = 10;
        //
        // if ( int(g_count / 8000) % 2 == 0 ) step = 10;
        // else step = -10;
        //
        // if ( g_count % 1000 == 0 ) {
        //     this->set_led_state( this->LED_STATE + step );
        //     this->draw_state_to_led();
        // }



    }



    // while (true)
    // {
    //

        // if (trigger > this->thresh[sens_i])
        // {
        //     write_all(this->SENS_PINS[sens_i], trigger);
        //
        //     if ( ping_flag && (abs(prev_hit - sens_i) == 1) ) {
        //         momentum = sens_i - prev_hit;
        //         this->adjust_led(momentum);
        //     }
        //
        //     // restrict sample pins to +-1 pin centered on sens_i
        //     bot_limit = max(sens_i - 1, 0);
        //     top_limit = min(sens_i + 1, this->NUM_SENS - 1);
        //
        //     ping_count = 0;
        //     ping_flag = true;
        //
        //     prev_hit = sens_i;
        //
        // } else if (ping_flag) {
        //     ping_count++;
        //
        //     if (ping_count > ping_timeout) {
        //         bot_limit = 0;
        //         top_limit = this->NUM_SENS - 1;
        //
        //         ping_count = 0;
        //         ping_flag = false;
        //
        //         momentum = 0;
        //
        //         Serial.println("ping_flag unset");
        //     }
        // }


        // scanning buttons from bot_limit to top_limit
//         if (flag_up) sens_i++;
//         else sens_i--;
//
//         if (sens_i > top_limit) {
//             sens_i -= 2;
//             sens_i = this->clamp_index_bounds(sens_i);
//             flag_up = false;
//         }
//         if (sens_i < bot_limit) {
//             sens_i += 2;
//             sens_i = this->clamp_index_bounds(sens_i);
//             flag_up = true;
//         }
//
//     }  // while


}

void TouchSlider::slider_old()
{
    int sens_i = 0;
    int bot_limit = 0;
    int top_limit = this->NUM_SENS - 1;

    bool flag_up = true;

    int ping_count = 0;
    bool ping_flag = false;
    const int ping_timeout = 100;  // in number of samples

    float momentum = 0;
    int prev_hit = 0;


    while (true)
    {
        int trigger = sample_pin(sens_i, 5);

        if (trigger > this->thresh[sens_i])
        {
            write_all(this->SENS_PINS[sens_i], trigger);

            if ( ping_flag && (abs(prev_hit - sens_i) == 1) ) {
                momentum = sens_i - prev_hit;
                // this->adjust_led(momentum);
            }

            // restrict sample pins to +-1 pin centered on sens_i
            bot_limit = max(sens_i - 1, 0);
            top_limit = min(sens_i + 1, this->NUM_SENS - 1);

            ping_count = 0;
            ping_flag = true;

            prev_hit = sens_i;

        } else if (ping_flag) {
            ping_count++;

            if (ping_count > ping_timeout) {
                bot_limit = 0;
                top_limit = this->NUM_SENS - 1;

                ping_count = 0;
                ping_flag = false;

                momentum = 0;

                Serial.println("ping_flag unset");
            }
        }


        // scanning buttons from bot_limit to top_limit
        if (flag_up) sens_i++;
        else sens_i--;

        if (sens_i > top_limit) {
            sens_i -= 2;
            sens_i = clamp_index_bounds(sens_i);
            flag_up = false;
        }
        if (sens_i < bot_limit) {
            sens_i += 2;
            sens_i = clamp_index_bounds(sens_i);
            flag_up = true;
        }

    }  // while

}

// for 1 sensor
void TouchSlider::debug()
{
    const int thresh_up = 5;
    const int thresh_dn = -6;
    
    float val_av = 0;
    const float alpha = 0.01;
    const float alpha_inv = 1 - alpha;
    
    float diff_av = 0;

    while (true)
    {
        // float new_val = analogRead( this->SENS_PINS[0] );
        float new_val = sample_pin(0, 10);

        float new_diff = abs(new_val - val_av);

        val_av = alpha * new_val + alpha_inv * val_av;
        val_av = max(val_av, 0);
        
        diff_av = alpha * new_diff + alpha_inv * diff_av;
        diff_av = max(diff_av, 0);
        
        float diff_2 = new_diff - diff_av;

        Serial.print(new_val);
        Serial.print(" ");
        Serial.print(val_av);
        Serial.print(" ");
        
        Serial.print(" ");
        Serial.print(new_diff);
        Serial.print(" ");
        Serial.print(diff_av);
        
        Serial.print(" ");
        Serial.print(diff_2);
        
        Serial.print(0);
        Serial.print(" ");
        Serial.print(100);
        
        Serial.println();
        
        // Serial.print(" ");    
        // Serial.println((new_diff > thresh_up) | (new_diff < thresh_dn) ? 50 : 0 );
        
        // int show = 0;
        // if (new_diff > thresh_up) show = 10;
        // if (new_diff < thresh_dn) show = -10;
        // Serial.println(show);
    }
}


// for 1 sensor
void TouchSlider::debug_windowed()
{
    float run_av = 0;
    const int window_size = 20;

    float val_buff[window_size] = {0};
    int buff_i = 0;

    while (true)
    {
        float new_val = analogRead( this->SENS_PINS[0] );

        float oldest_val = val_buff[buff_i];
        val_buff[buff_i] = new_val;

        buff_i++;
        if (buff_i >= window_size) buff_i = 0;

        run_av = run_av + ( (new_val - oldest_val) / float(window_size) );

        float trigger = run_av;
        float trigger2 = new_val;

        Serial.print(trigger);
        Serial.print(" ");
        Serial.print(trigger2);
        Serial.print(" ");
        Serial.print(0);
        Serial.print(" ");
        Serial.println(100);
    }

}












