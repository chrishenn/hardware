#!/usr/bin/env python

from smartcard.CardType import ATRCardType
from smartcard.CardConnection import CardConnection
from smartcard.CardRequest import CardRequest
from smartcard.util import toHexString, toBytes
import time

def comment(i):
  try:
    return {
      0:'ATR',
      1:'Fab Code | MTZ | Card Manuf.',
      2:'Lot History Code',
      3:'DCR | ID Number Nc',
      4:'AR0 | PR0 | AR1...',
      5:'Reserved',
      6:'Reserved',
      7:'Reserved',
      8:'Issuer Code',
      9:'Issuer Code',
      10:'AAC0 | Crytpogram C0',
      11:'Session Key S0',
      12:'AAC1 | Crytpogram C1',
      13:'Session Key S1',
      14:'AAC2 | Crytpogram C2',
      15:'Session Key S2',
      16:'AAC3 | Crytpogram C3',
      17:'Session Key S3',
      18:'Seed G0',
      19:'Seed G1',
      20:'Seed G2',
      21:'Seed G3',
      22:'PAC | Write0 PW | PAC | Read0 PW',
      23:'PAC | Write1 PW | PAC | Read1 PW',
      24:'PAC | Write2 PW | PAC | Read2 PW',
      25:'PAC | Write3 PW | PAC | Read3 PW',
      26:'PAC | Write4 PW | PAC | Read4 PW',
      27:'PAC | Write5 PW | PAC | Read5 PW',
      28:'PAC | Write6 PW | PAC | Read6 PW',
      29:'PAC | Write7 PW | PAC | Read7 PW',
    }[i]
  except:
    return ''

def print_confdump(resp):
  for i in range(len(resp)/8):
    print toHexString(resp[8*i:8*(i+1)]), '=>', comment(i)

def confdump(apdu):
  print 'sending ' + toHexString(apdu)
  apdu = [0x00]+apdu

  response, sw1, sw2 = cardservice.connection.transmit( apdu, CardConnection.T0_protocol )
  print 'response: '
  print_confdump(response)
  print 'status words: ', "%x %x" % (sw1, sw2)
  print

def print_memdump(resp):
  for i in range(len(resp)/8):
    print toHexString(resp[8*i:8*(i+1)])

def sendapdu(apdu):
  print 'sending ' + toHexString(apdu)
  apdu = [0x00]+apdu

  response, sw1, sw2 = cardservice.connection.transmit( apdu, CardConnection.T0_protocol )
  print 'response: '#, toHexString(response)
  print_memdump(response)
  print 'status words: ', "%x %x" % (sw1, sw2)
  print

def read_conf():
  print 'Reading configuration'
  READCONF = [0xB6, 0x00, 0x00, 0xF0]
  confdump(READCONF)

def write_ar(zone, data):
  print 'Setting AR', zone, 'to', toHexString([data])
  addr = 0x20 + 2*zone
  apdu = [0xB4, 0x08, addr, 0x01, data]
  sendapdu(apdu)

def write_pr(zone, data):
  print 'Setting PR', zone, 'to', toHexString([data])
  addr = 0x20 + 2*zone + 1
  apdu = [0xB4, 0x08, addr, 0x01, data]
  sendapdu(apdu)

def sel_zone(zone):
  print 'Selecting zone', zone
  apdu = [0xB4, 0x0B, zone] #oddly enough, this causes a hair-tearing error if you send an N byte
  sendapdu(apdu)

def get_zone():
  print 'Reading zone'
  apdu = [0xB2, 0x00, 0x00, 0x80]
  print 'sending ' + toHexString(apdu)
  apdu = [0x00]+apdu

  response, sw1, sw2 = cardservice.connection.transmit( apdu, CardConnection.T0_protocol )
  print 'status words:', "%x %x" % (sw1, sw2)
  return response

def read_zone(zone):
  sel_zone(zone)
  print_memdump(get_zone())
  print

def save_zone_to_file(zone,fname):
  print 'Saving zone', zone, 'to', str(fname)+':'
  sel_zone(zone)
  f = open(fname, 'w')
  f.write(' '.join(map(str,get_zone())))
  print

def load_zone_to_card(zone,fname):
  print 'Loading', fname, 'to zone', str(zone)+':'
  sel_zone(zone)
  f = open(fname, 'r')
  data = map(int,f.readline().split(' '))
  WRITESIZE = 0x08
  for i in range(0x80/WRITESIZE):
    WRITE = [0xB0, WRITESIZE*i, WRITESIZE*i, WRITESIZE]
    sendapdu(WRITE+data[WRITESIZE*i:WRITESIZE*(i+1)])

#program starts here
cardtype = ATRCardType( toBytes( "3B B2 11 00 10 80 00 04" ) ) #require a crytpomemory 404c
cardrequest = CardRequest( timeout=1, cardType=cardtype )
cardservice = cardrequest.waitforcard()

cardservice.connection.connect()

VERIFY = [0xBA, 0x07, 0x00, 0x03,]
PWD = [0x60, 0x57, 0x34]
print 'Authorizing:'
sendapdu(VERIFY+PWD)

print 'Writing AR:'
write_ar(0x00, 0xFF)
write_ar(0x01, 0xFF)

save_zone_to_file(0x00, 'zone0_backup.bin')
save_zone_to_file(0x01, 'zone1_backup.bin')
load_zone_to_card(0x00, 'zone0_backup.bin')
load_zone_to_card(0x01, 'zone1_backup.bin')

print 'Restoring AR:'
write_ar(0x00, 0xDF)
write_ar(0x01, 0xDF)

exit()
#program ends here

def read_fuse():
  READFUSE = [0xB6, 0x01, 0x00, 0x01]
  sendapdu(READFUSE)
#print 'Reading fuse:'
#read_fuse()

def read_cksum():
  READSUM = [0xB6, 0x02, 0x00, 0x02]
  sendapdu(READSUM)
#print 'Reading checksum:'
#read_cksum()
