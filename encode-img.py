import base64

File = "/Users/trevor-trou/Documents/Git/snow-model/Evaluate/NoSnow/RoadSurface2019-02-15_10-15-02.jpg"

with open(File, "rb") as image_file:
    encoded_string = base64.b64encode(image_file.read())

resultFile = open("EncodedImg.txt", "a")
resultFile.write(encoded_string)
resultFile.close()