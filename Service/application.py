from flask import Flask, request, Response
from model import evaluate
from decode import decodeAndSizeImg

import json

float_formatter = lambda x: float("%.4f" % x)

app = Flask(__name__)

@app.route('/', methods=['GET', 'POST'])
def hello_world():
    if request.method == 'GET':
        return 'Use POST'
    
    # if request.content_type != 'application/json':
    #     return Response("", status=415)

    try:
        base64Img = json.loads(request.data)
    except ValueError:
        return Response('Body must be of the form: { image: "base64EncodedImage" }', status=400)

    if 'image' not in base64Img:
        return Response('Body must be of the form: { image: "base64EncodedImage" }', status=400)

    try:
        img = decodeAndSizeImg(base64Img['image'])
    except:
        return Response('Unable to decode image. Invalide base64 string.', status=400)

    prediction = evaluate(img)

    if prediction > 0.5:
        result = True
        confidence = prediction
    else:
        result = False
        confidence = 1 - prediction

    response = json.dumps({'snowOnRoad': result, 'confidence': float_formatter(confidence)})

    return Response(response, status=200, mimetype='application/json')

if __name__ == '__main__':
    app.run(debug=False, port=80)