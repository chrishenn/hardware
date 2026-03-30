#include <ArduinoOTA.h>
#include <ESPmDNS.h>
#include <NetworkUdp.h>
#include <WiFi.h>

#include "public_key.h"

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

void setup_blink() {
    pinMode(LED_BUILTIN, OUTPUT);
}

void setup() {
    setup_wifi();
    setup_ota();
    setup_blink();
    xTaskCreatePinnedToCore(loop_blink, "loop_blink", 4096, NULL, 1, NULL, 0);
}

void loop_blink(void *params) {
    const int blink_interval = 200;

    while (true) {
        digitalWrite(LED_BUILTIN, HIGH);
        delay(blink_interval);
        digitalWrite(LED_BUILTIN, LOW);
        delay(blink_interval);
    }
}

void loop() {
    ArduinoOTA.handle();
}
