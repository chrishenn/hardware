# ADC Calibration Using ESP32’s Built-in eFuse Data

source: https://zbotic.in/esp32-adc-accuracy-fix-noise-and-non-linearity-issues/

---

Espressif provides an ADC Calibration API in the ESP-IDF framework (also available in Arduino via esp_adc_cal.h). Each
ESP32 chip is factory-calibrated with coefficients stored in eFuse that correct for the non-linearity curve. Using this
API dramatically improves accuracy:

```cpp
#include <esp_adc_cal.h>
#include <driver/adc.h>

#define ADC_CHANNEL ADC1_CHANNEL_6 // GPIO34
#define ADC_ATTEN   ADC_ATTEN_DB_11

esp_adc_cal_characteristics_t adc_chars;

void setup() {
    Serial.begin(115200);
    adc1_config_width(ADC_WIDTH_BIT_12);
    adc1_config_channel_atten(ADC_CHANNEL, ADC_ATTEN);

    // Characterise ADC using eFuse coefficients
    esp_adc_cal_characterize(
        ADC_UNIT_1,
        ADC_ATTEN,
        ADC_WIDTH_BIT_12,
        1100, // Default Vref (mV), ignored if eFuse available
        &adc_chars
    );
}

void loop() {
    // Average 64 samples
    uint32_t adc_reading = 0;
    for (int i = 0; i < 64; i++) {
        adc_reading += adc1_get_raw(ADC_CHANNEL);
    }
    adc_reading /= 64;

    // Convert to calibrated millivolts
    uint32_t voltage_mv = esp_adc_cal_raw_to_voltage(adc_reading, &adc_chars);
    Serial.printf("Raw: %d | Voltage: %d mV (%.3f V)n", adc_reading, voltage_mv, voltage_mv / 1000.0f);
    delay(500);
}
```

This calibrated approach typically reduces voltage reading error from ±100–150mV to ±20–30mV, a 5x improvement without
any additional hardware.

The esp_adc_cal.h library from Espressif is the standard approach and is included in the ESP32 Arduino board package.
There is also the ESP32AnalogRead library on GitHub that wraps the calibration API with a simple getValue() and
getMilliVolts() interface. Both are good choices for cleaner calibrated readings.

---

When to Use an External ADC (ADS1115)

For applications that need truly accurate analog readings — precision weighing, medical sensors, high-resolution
audio — no amount of software tricks will make the ESP32’s internal ADC sufficient. In these cases, use an external ADC
over I2C or SPI.

The ADS1115 (Texas Instruments) is the most popular choice:

- 16-bit resolution (vs ESP32’s effective 10–11 bits)
- Programmable gain amplifier (±256mV to ±6.144V ranges)
- I2C interface (plug into any ESP32 I2C pins)
- 4 single-ended or 2 differential inputs
- Integrated voltage reference (no noise from ESP32 power rail)
