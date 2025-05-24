import time
import urllib.parse
import hmac
import hashlib
import base64

def generate_sas_token(uri, key, expiry=3600):
    ttl = int(time.time()) + expiry
    sign_key = "%s\n%d" % ((urllib.parse.quote_plus(uri)), ttl)
    key = base64.b64decode(key)
    signature = base64.b64encode(hmac.new(key, sign_key.encode('utf-8'), hashlib.sha256).digest())
    signature = urllib.parse.quote_plus(signature)
    token = f"SharedAccessSignature sr={urllib.parse.quote_plus(uri)}&sig={signature}&se={ttl}"
    return token

connection_string = "HostName=wirtualnygaraz.azure-devices.net;DeviceId=simulatorgarage;SharedAccessKey=mzODsK0gtsXQLnNfb+FNN0/2XC8gwROdmEmahp50NB8="
parts = dict(item.split('=', 1) for item in connection_string.split(';'))

uri = f"{parts['HostName']}/devices/{parts['DeviceId']}"
key = parts['SharedAccessKey']

sas_token = generate_sas_token(uri, key, expiry=3600) 
print("SAS Token:", sas_token)