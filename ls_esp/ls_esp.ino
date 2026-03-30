#include <Arduino.h>
#include <ArduinoOTA.h>
#include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>
#include <ESPmDNS.h>
#include <NetworkUdp.h>
#include <WebSerial.h>
#include <WiFi.h>
#include <esp_task_wdt.h>

#include "public_key.h"

const int pin_sense = 1;
const int pin_relay = 2;

const String wifi_ssid = "Coop_2";
const String wifi_pass = "";
const String ota_host  = "ls_esp32";
const String ota_pass  = "";

// this must be created in global scope
AsyncWebServer server(80);

void setup_wdt() {
    // disable idle wdt; set wdt timeout to 10s for tasks (wifi will not reset the watchdog until connected)
    esp_task_wdt_config_t twdt_config = {
        .timeout_ms     = 10000,
        .idle_core_mask = 0,
        .trigger_panic  = false,
    };
    esp_task_wdt_deinit();
    esp_task_wdt_init(&twdt_config);
}

void setup_wifi() {
    Serial.printf("start wifi connect\n");
    WiFi.mode(WIFI_STA);
    WiFi.begin(wifi_ssid.c_str(), wifi_pass.c_str());
    while (WiFi.waitForConnectResult() != WL_CONNECTED) {
        delay(1000);
        ESP.restart();
    }
    Serial.printf("IP address: %s\n", WiFi.localIP().toString());
}

void setup_ota() {
    int hashType = HASH_SHA512;

    static UpdaterECDSAVerifier sign(PUBLIC_KEY, PUBLIC_KEY_LEN, hashType);
    ArduinoOTA.setSignature(&sign);
    ArduinoOTA.setHostname(ota_host.c_str());
    ArduinoOTA.setPassword(ota_pass.c_str());

    ArduinoOTA
        .onStart([]() {
            String type;
            if (ArduinoOTA.getCommand() == U_FLASH) {
                type = "sketch";
            } else {
                type = "filesystem";
            }
        })
        .onEnd([]() {})
        .onProgress([](unsigned int progress, unsigned int total) {})
        .onError([](ota_error_t error) {
            if (error == OTA_AUTH_ERROR) {
            } else if (error == OTA_BEGIN_ERROR) {
            } else if (error == OTA_CONNECT_ERROR) {
            } else if (error == OTA_RECEIVE_ERROR) {
            } else if (error == OTA_END_ERROR) {
            }
        });
    ArduinoOTA.begin();
}

void setup_ls() {
    // default: 12 (read value range: 0-4096). Possible resolution bit widths: {9, 10, 11, 12}
    // analogReadResolution(12);

    // default analog pin attenuation: ADC_11db. Set lower for greater sensitivity at low sensor voltages
    // values: {ADC_0db, ADC_2_5db, ADC_6db, ADC_11db}
    analogSetAttenuation(ADC_0db);

    pinMode(pin_sense, INPUT);
    pinMode(pin_relay, OUTPUT);
    digitalWrite(pin_relay, LOW);
}

void prnt(HWCDC *s0, WebSerialClass *s1, float val0, float val1, float val2, float val3, float val4) {
    s0->printf("%8.1f,%8.1f,%8.1f,%8.1f,%8.1f\n", val0, val1, val2, val3, val4);
    s1->printf("%8.1f,%8.1f,%8.1f,%8.1f,%8.1f\n", val0, val1, val2, val3, val4);
}

void setup_webserial() {
    WebSerial.onMessage([](const String &msg) { Serial.println(msg); });
    WebSerial.begin(&server);
    server.onNotFound([](AsyncWebServerRequest *request) { request->redirect("/webserial"); });
    server.begin();
}

// void loop_ls(void *params) {
//     setup_webserial();
//
//     // after trigger_window reads, we check for any change great enough to exceed the trigger threshold
//     const int trigger_window = 5000;
//
//     // debounce is counted in sample-loops - same as the number of reads defined by trigger_window
//     int       debounce_timer   = 0;
//     const int debounce_timeout = 4 * trigger_window;
//
//     // an exponentially-decayed moving average is cheap, and does a reasonable job smoothing the signal
//     const float exp_smooth     = 0.01;
//     const float exp_smooth_inv = 1.0 - exp_smooth;
//     float       exp_val        = 0.0;
//     float       exp_prev       = 0.0;
//     float       exp_delta      = 0.0;
//
//     // trigger_thresh is set to a fraction of the value of the smoothed light reading
//     // when values are small, they are also noisy; hence the trigger value floor `trigger_min`
//     const float trigger_frac   = 0.40;
//     const float trigger_min    = 30;
//     float       trigger_thresh = 0.0;
//
//     bool trigger_fired = false;
//     bool relay_state   = false;
//
//     while (true) {
//         for (int i = 0; i < trigger_window; i++) {
//             exp_val = (exp_smooth * float(analogRead(pin_sense))) + (exp_smooth_inv * exp_val);
//         }
//         debounce_timer += trigger_window;
//
//         trigger_thresh = exp_prev * trigger_frac;
//         trigger_thresh = _max(trigger_thresh, trigger_min);
//
//         exp_delta = abs(exp_val - exp_prev);
//         exp_prev  = exp_val;
//
//         if ((exp_delta >= trigger_thresh) && (debounce_timer >= debounce_timeout)) {
//             trigger_fired = true;
//         }
//         prnt(&Serial, &WebSerial, exp_val, exp_delta, trigger_thresh, trigger_fired * 100);
//         WebSerial.loop();
//
//         if (trigger_fired) {
//             trigger_fired  = false;
//             debounce_timer = 0;
//             relay_state    = !relay_state;
//
//             if (relay_state) {
//                 digitalWrite(pin_relay, HIGH);
//             } else {
//                 digitalWrite(pin_relay, LOW);
//             }
//         }
//     }
// }

void loop_ota(void *params) {
    // this will not work when pinned to a core
    while (true) {
        ArduinoOTA.handle();
    }
}

volatile bool flag_adc_read = false;

void ARDUINO_ISR_ATTR adc_read_cb() {
    flag_adc_read = true;
}

int adc_poll(adc_continuous_result_t *adc_results) {
    while (true) {
        if (!flag_adc_read) {
            continue;
        }
        flag_adc_read = false;

        if (!analogContinuousRead(&adc_results, 0)) {
            Serial.println("ERROR: ADC read flag set, but results were not read into results array");
            continue;
        }
        return adc_results[0].avg_read_raw;
    }
}

void loop_ls_cont(void *params) {
    setup_webserial();

    // after trigger_window reads, we check for any change great enough to exceed the trigger threshold
    const int trigger_window = 1000;

    // debounce is counted in sample-loops - same as the number of reads defined by trigger_window
    int       debounce_timer   = 0;
    const int debounce_timeout = 5 * trigger_window;

    // an exponentially-decayed moving average is cheap, and does a reasonable job smoothing the signal
    const float exp_smooth     = 0.5;
    const float exp_smooth_inv = 1.0 - exp_smooth;
    float       exp_val        = 0.0;
    float       exp_prev       = 0.0;

    float exp_delta      = 0.0;
    float exp_delta_prev = 0.0;
    float sum_response   = 0.0;

    // trigger_thresh is set to a fraction of the value of the smoothed light reading
    // when values are small, they are also noisy; hence the trigger value floor `trigger_min`
    const float trigger_frac   = 0.40;
    const float trigger_min    = 30;
    float       trigger_thresh = 0.0;

    bool trigger_fired = false;
    bool relay_state   = false;

    // continuous analog read setup.
    // continuous reads are slightly faster than one-shot reads for the same (large) number of samples
    // ~240 reports/min over webserial at adc_freq 20000, for 5000 samples per report (trigger_window*adc_samples)
    // the adc appears to be the bottleneck, since settings higher adc_freq gives faster webserial reports
    const uint8_t pins_adc[]   = {pin_sense};
    const uint8_t pins_adc_len = sizeof(pins_adc) / sizeof(uint8_t);
    const int     adc_samples  = 10;

    // max sample frequency is ~ 2M/s. Is there a disadvantage to faster sampling?
    const int                adc_freq    = SOC_ADC_SAMPLE_FREQ_THRES_HIGH / 2;
    adc_continuous_result_t *adc_results = NULL;

    pinMode(pin_sense, INPUT);
    pinMode(pin_relay, OUTPUT);
    digitalWrite(pin_relay, LOW);

    analogContinuousSetAtten(ADC_0db);
    analogContinuous(pins_adc, pins_adc_len, adc_samples, adc_freq, &adc_read_cb);
    analogContinuousStart();

    while (true) {
        for (int i = 0; i < trigger_window; i++) {
            exp_val = (exp_smooth * float(adc_poll(adc_results))) + (exp_smooth_inv * exp_val);
        }
        debounce_timer += trigger_window;

        trigger_thresh = exp_prev * trigger_frac;
        trigger_thresh = _max(trigger_thresh, trigger_min);

        exp_delta = abs(exp_val - exp_prev);
        exp_prev  = exp_val;

        sum_response   = exp_delta + exp_delta_prev;
        exp_delta_prev = exp_delta;

        if ((sum_response >= trigger_thresh) && (debounce_timer >= debounce_timeout)) {
            trigger_fired = true;
        }
        prnt(&Serial, &WebSerial, exp_val, exp_delta, sum_response, trigger_thresh, trigger_fired * 100);
        WebSerial.loop();

        if (!trigger_fired) {
            continue;
        }

        trigger_fired  = false;
        debounce_timer = 0;
        relay_state    = !relay_state;
        if (relay_state) {
            digitalWrite(pin_relay, HIGH);
        } else {
            digitalWrite(pin_relay, LOW);
        }
    }
}

void setup() {
    Serial.begin(115200);

    setup_wdt();
    //     setup_ls();
    setup_wifi();
    setup_ota();

    // OTA task has lower priority
    // disable watchdog timer on idle task to use this setup - otherwise the wdt on core0's idle task will panic
    xTaskCreate(loop_ota, "loop_ota", 4096, NULL, 1, NULL);
    //     xTaskCreate(loop_ls, "loop_ls", 4096, NULL, 2, NULL);
    xTaskCreate(loop_ls_cont, "loop_ls_cont", 4096, NULL, 2, NULL);
}

void loop() {}
