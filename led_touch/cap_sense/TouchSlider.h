
#ifndef TOUCHSLIDER_H
#define TOUCHSLIDER_H

#include "Arduino.h"


class TouchSlider
{
    private:

        int* noise;
        int* thresh;

        int NUM_SENS;
        int* SENS_PINS;

        int LED_PIN;
        int LED_STATE;
        int LED_STEP;

        void set_led_state(int state);
        int perc_to_led_int(int perc_state);
        void draw_state_to_led();
        int clamp_index_bounds(int sens_i);

        void slider_old();

    public:
        TouchSlider( const int NUM_SENS, const int* SENS_PINS, const int LED_PIN );
        ~TouchSlider();

        void calibrate();
        void debug();
        void slider();
        float sample_pin(const int sens_i, const int n_samples );

        // void debug_exp_avg();
        void debug_windowed();
};

#endif
