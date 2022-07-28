from PIL import Image
from io import BytesIO
import base64

def decodeAndSizeImg(data):
    img = Image.open(BytesIO(base64.b64decode(data)))
    img = img.resize((150,150))
    return img