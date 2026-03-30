#include <Arduino.h>
#include <ArduinoOTA.h>
#include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>
#include <ESPmDNS.h>
#include <NetworkUdp.h>
#include <WebSerial.h>
#include <WiFi.h>

#include "public_key.h"

const int pin_sense = 32;
const int pin_relay = 33;

const String wifi_ssid = "Coop_2";
const String wifi_pass = "";
const String ota_host  = "ls_esp32";
const String ota_pass  = "";

// afaik, this must be declared in global scope
AsyncWebServer server(80);

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

void prnt(HardwareSerial *s0, WebSerialClass *s1, float val0, float val1, float val2, float val3) {
    s0->printf("%6.1f,%6.1f,%6.1f,%6.1f\n", val0, val1, val2, val3);
    s1->printf("%6.1f,%6.1f,%6.1f,%6.1f\n", val0, val1, val2, val3);
}

void setup_webserial() {
    WebSerial.onMessage([](const String &msg) { Serial.println(msg); });
    WebSerial.begin(&server);
    server.onNotFound([](AsyncWebServerRequest *request) { request->redirect("/webserial"); });
    server.begin();
}

void loop_ls(void *params) {
    // after trigger_window reads, we check for any change great enough to exceed the trigger threshold
    const int trigger_window = 5000;

    int       debounce_timer   = 0;
    const int debounce_timeout = 4 * trigger_window;

    // an exponentially-decayed moving average is cheap, and does a reasonable job smoothing the signal
    const float exp_smooth     = 0.01;
    const float exp_smooth_inv = 1.0 - exp_smooth;
    float       exp_av         = 0.0;
    float       exp_prev       = 0.0;
    float       exp_delta      = 0.0;

    // the trigger_thresh is set to a bounded fraction of the value of the smoothed light reading
    // when values are small, they are also very noisy. hence the minimum trigger_frac
    const float trigger_frac   = 0.40;
    const float trigger_min    = 30;
    float       trigger_thresh = 0.0;

    bool trigger_fired = false;
    bool relay_state   = false;
    int  val           = 0;
    setup_webserial();

    while (1) {
        for (int i = 0; i < trigger_window; i++) {
            exp_av = (exp_smooth * float(analogRead(pin_sense))) + (exp_smooth_inv * exp_av);
        }
        debounce_timer += trigger_window;

        trigger_thresh = exp_prev * trigger_frac;
        trigger_thresh = _max(trigger_thresh, trigger_min);

        exp_delta = abs(exp_av - exp_prev);
        exp_prev  = exp_av;

        if ((exp_delta >= trigger_thresh) && (debounce_timer >= debounce_timeout)) {
            trigger_fired = true;
        }

        // removing this delay will cause ota updates to fail, and cause webserial to stutter unusably
        prnt(&Serial, &WebSerial, exp_av, exp_delta, trigger_thresh, trigger_fired * 100);
        WebSerial.loop();
        delay(10);

        if (trigger_fired) {
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
}

void setup() {
    // default: 12-bit analogRead range: 0-4096. values: {9, 10, 11, 12}
    analogReadResolution(12);
    // default analog pin attenuation: ADC_11db. Set lower for greater sensitivity at low sensor voltages
    // values: {ADC_0db, ADC_2_5db, ADC_6db, ADC_11db}
    analogSetAttenuation(ADC_0db);
    Serial.begin(115200);

    pinMode(pin_sense, INPUT);
    pinMode(pin_relay, OUTPUT);
    digitalWrite(pin_relay, LOW);

    setup_wifi();
    setup_ota();

    xTaskCreatePinnedToCore(loop_ls, "loop_ls", 4096, NULL, 1, NULL, 0);
}

void loop() {
    // this cannot go into a task pinned to core
    ArduinoOTA.handle();
}
