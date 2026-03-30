# ls_esp: ESP32 Remote Light Switch

# idea

Use the existing light switch in the storage closet to turn on the storage closet light. And, use an esp32 with an
ambient light sensor to detect that the closet light has been turned on, to also turn on supplemental LEDs in the
storage closet.

Note: I know almost nothing about electronics, so don't assume I know what I'm doing and blindly copy what's happening
in this diagram

<p align="center">
  <img src="https://raw.githubusercontent.com/chrishenn/hardware/refs/heads/main/ls_esp/doc/schematic.png"  alt=""/>
</p>

# status

Working! This setup responds to the storage closet light being switched on, and in reponse, switches on the LEDs in
the closet. In addition, the microcontroller ignores flipping on other lights in the house (as expected).

Critical rule: Never use ADC2 pins (GPIO0, GPIO2, GPIO4, GPIO12–15, GPIO25–27) when Wi-Fi is active. ADC2 is shared with
the Wi-Fi radio hardware and will give garbage readings or cause conflicts. Stick to ADC1 pins (GPIO32–39) for all
analog measurements.

# todo

- [x] (local) serial graphing
    - https://web-serial-plotter.atomic14.com/
    - https://serialplotter.io/
- [x] OTA updates
    - arduinoOTA, arduino's built-in example
- [x] remote value debugging
    - webserial
- [x] 5V step-down power supply module
    - the LM7805 got really hot - it's officially retired for me
- [x] mosfet switching circuit
    - [x] optocoupler
- [x] microcontroller
    - [x] esp-wroom-32 right now
    - [x] xiao esp32S3_plus
- [x] triggering algo
- [x] implement continuous ADC sampling
- [x] handle watchdog timeout on core0's idle task
- [ ] remote value graphing
    - https://esp32io.com/tutorials/esp32-web-plotter
- [ ] integrate this circuit onto a custom PCB

# usage

note: tested only linux

```bash
$ just -l
Available recipes:
    bp      # build + push: build, sign, and push a signed binary over arduinoOTA
    build   # build and sign a binary [alias: b]
    clean   # remove builds, bins, and generated keys [alias: c]
    connect # connect over bluetooth serial (note: I turned this off)
    init    # generate initial keypair for OTA binary signing
    push    # push a signed binary over arduinoOTA [alias: p]

# eg
just build
just push
```

# manual build and sign, with options

```bash
#define USE_RSA
python sign.py --generate-key rsa-2048 --out private_key.pem
python sign.py --generate-key rsa-3072 --out private_key.pem
python sign.py --generate-key rsa-4096 --out private_key.pem

#define USE_ECDSA
python sign.py --generate-key ecdsa-p256 --out private_key.pem
python sign.py --generate-key ecdsa-p384 --out private_key.pem

# extract public key, generate public_key.h
python sign.py --extract-pubkey private_key.pem --out public_key.pem

# show board properties
arduino-cli board details -b esp32:esp32:XIAO_ESP32S3_Plus | grep -i partitionscheme
#    Partition Scheme                            PartitionScheme
#    16M Flash (2MB APP/12.5MB FATFS)            PartitionScheme=fatflash
#    16M Flash (3MB APP/9.9MB FATFS)             PartitionScheme=app3M_fat9M_16MB
#    Default with spiffs (3MB APP/1.5MB SPIFFS)  PartitionScheme=default_8MB
#    Maximum APP (7.9MB APP No OTA/No FS)        PartitionScheme=max_app_8MB
#    TinyUF2 8MB (2MB APP/3.7MB FFAT)            PartitionScheme=tinyuf2

# with details
arduino-cli board details -b esp32:esp32:XIAO_ESP32S3_Plus --show-properties=expanded
#    menu.PartitionScheme.fatflash.build.partitions=ffat
#    menu.PartitionScheme.fatflash.upload.maximum_size=2097152
#    menu.PartitionScheme.fatflash=16M Flash (2MB APP/12.5MB FATFS)
#    menu.PartitionScheme.max_app_8MB.build.partitions=max_app_8MB
#    menu.PartitionScheme.max_app_8MB.upload.maximum_size=8257536
#    menu.PartitionScheme.max_app_8MB=Maximum APP (7.9MB APP No OTA/No FS)

# we can manually specify build.partitions and upload.maximum_size as --build-property's:

# build bin (esp32_wroom)
arduino-cli compile --export-binaries \
  --fqbn esp32:esp32:esp32da \
  --build-property build.partitions=min_spiffs \
  --build-property upload.maximum_size=1966080 \
  --build-path build \
  ls_esp.ino

# or, the recommended way: add the scheme's option name into the board triple (quadruple?)

# build bin (note that this is the default partitionscheme in arduino IDE for XIAO_ESP32S3_Plus)
arduino-cli compile --export-binaries \
  --fqbn esp32:esp32:XIAO_ESP32S3_Plus:PartitionScheme=fatflash \
  --build-path build \
  ls_esp.ino

# build bin (esp32_wroom). Note that min_spiffs scheme is needed for OTA to work on esp32_wroom
arduino-cli compile --export-binaries \
  --fqbn esp32:esp32:esp32da:PartitionScheme=min_spiffs \
  etc

# these options are meant to be comma-separated, ie
arduino-cli upload -p /dev/usb0 -b esp32:esp32:esp32:FlashMode=qio,UploadSpeed=115200,PartitionScheme=no_ota project.ino

# sign bin
python sign.py --bin build/ls_esp.ino.bin --key private_key.pem --out signed.bin --hash sha256
python sign.py --bin build/ls_esp.ino.bin --key private_key.pem --out signed.bin --hash sha384
python sign.py --bin build/ls_esp.ino.bin --key private_key.pem --out signed.bin --hash sha512

# upload signed bin
python espota.py -i <ip> -f signed.bin [-a <ota_password>]
```

### board init

- generate keys
- Tools → Partition Scheme → Minimal SPIFFS (1.9MB APP with OTA)
- compile and upload this sketch over USB

```bash
# generate keys
python sign.py --generate-key rsa-2048 --out private_key.pem
python sign.py --extract-pubkey private_key.pem --out public_key.pem
```

### build and sign

```bash
# Using Arduino IDE
# Tools → Partition Scheme → Minimal SPIFFS (1.9MB APP with OTA)
# In Arduino IDE: Sketch → Export Compiled Binary

# find the build triple for my board
arduino-cli core update-index
arduino-cli core install esp32:esp32
arduino-cli core list
arduino-cli board list
arduino-cli board listall esp32 | grep -i wroom
arduino-cli board listall esp32 | grep -i xiao

# or, using cli (see summary)

# sign
python sign.py --bin build/ls_esp.ino.bin --key private_key.pem --out firmware_signed.bin --hash sha256
```

### upload

```bash
# you can connect to the esp over the network - should show under the board drop-down using the ip of the server
# if the board's ip doesn't show, try closing and re-opening the arduino ide
# then you can just hit the gui upload button, and use the ota_password in the gui dialog box

# or, using espota.py (omit -a <ota_password> if your sketch set ota_password = nullptr):
python espota.py -i "192.168.1.15" -f firmware_signed.bin -a <ota_password>
```

### bluetooth serial monitoring

note: this is janky, and so I turned it off

```bash
mac="F4:2D:C9:70:90:AE"
pin="401741"

sudo bluetoothctl
power on
agent on
scan on
pair $DEV_MAC

# make note of the pin. it's ok if you pair and then disconnect.
# close bluetoothctl.

sudo killall rfcomm
sudo rfcomm connect /dev/rfcomm0 $DEV_MAC 1 & disown

sudo apt install -y screen
sudo screen /dev/rfcomm0

# To quit, use: (CTRL + A) + :quit
```

you could also use serial.tools.miniterm - we would want to parse and graph the result to replicate the graphing serial
monitor from arduino ide

### partitions

In the min_spiffs.cvs file on the app0 line:
app0, app, ota_0, 0x10000, 0x1E0000,

The last number of 0x1E0000 is the hex equivalent of 1966080 which means, if you use another partitioning configuration
you should be able to find the number on the end of the app0 line and convert that from hex to decimal and the compiler
should work with that partitioning too.

### fixed problem

I fixed this issue by swapping the premade photoresistor module for a through-hole photoresistor with a 1K resistor, and
measuring at the voltage division between them. I experimented with resistors until ambient light consditions in the
target range provided readable values to the ESP32 ADC.

Problem: the bottom end of the ADC on ESP32 is totally flat. Turning on the light bulb in the storage closet adds ~40 to
~50 points to the measurement (out of 4096) - but crucially, only when there is other ambient light. When the room is
dark, turning on the storage light does not register.

I must have the house lights on in order for the storage light to register.
Even the kitchen lights are not enough to bring the sensor into measurable range.

<p align="center">
  <img src="https://raw.githubusercontent.com/chrishenn/hardware/refs/heads/main/ls_esp/doc/esp32_adc.png"  alt=""/>
</p>

### switch triggering algo

Typically we see a clean transition, where our exponentially-smoothed function (exp_av) changes within a trigger window,
registering a large delta among low delta values from start-of-window to end-of-window:

```csv
exp_av   delta  delta_2  threshold  fired

208.5,     1.9,     2.1,    84.1,     0.0
208.3,     0.1,     2.0,    83.4,     0.0
205.2,     3.2,     3.3,    83.3,     0.0
716.3,   511.1,   514.3,    82.1,   100.0
731.6,    15.3,   526.4,   286.5,     0.0
736.0,     4.4,    19.7,   292.6,     0.0
738.3,     2.3,     6.7,   294.4,     0.0
732.0,     6.3,     8.6,   295.3,     0.0
734.6,     2.6,     9.0,   292.8,     0.0
732.6,     2.0,     4.6,   293.8,     0.0
218.1,   514.5,   516.5,   293.0,   100.0
212.8,     5.3,   519.8,    87.2,     0.0
209.6,     3.2,     8.5,    85.1,     0.0
208.9,     0.7,     3.9,    83.8,     0.0
206.3,     2.6,     3.3,    83.6,     0.0
213.0,     6.7,     9.3,    82.5,     0.0
202.2,    10.8,    17.5,    85.2,     0.0
719.2,   517.0,   527.8,    80.9,   100.0
```

The smoothed function's response is relatively fast compared to the size of the trigger window. However, the end of a
trigger window can land in the middle of a quickly-changing slope, spreading the true size of the delta value over two
trigger window's. To handle this case, delta_2 sums the delta value from the previous window.

For this to work, it is necessary that the total delta from a light change event is reflected in exp_av func within the
span of two trigger windows - this is a reasonable assumption, due to the note above.

Note that this effectively reduces the size of the debounce window by one trigger window's width.

This rudimentary hysteresis correctly detects that the threshold has been exceeded in the edge case:

```csv
exp_av   delta  delta_2  threshold  fired

725.3,     1.2,     3.6,   290.6,     0.0
439.2,   286.1,   287.3,   290.1,     0.0
211.5,   227.7,   513.8,   175.7,   100.0
207.2,     4.3,   232.0,    84.6,     0.0
201.7,     5.5,     9.8,    82.9,     0.0
```
