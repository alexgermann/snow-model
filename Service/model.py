from keras.preprocessing import image #Library for preprocessing the image into a 4D tensor
from keras.models import load_model
import numpy as np

model = load_model('snow-model.h5')
model._make_predict_function()

def evaluate(img):
    img_tensor = image.img_to_array(img)
    img_tensor = np.expand_dims(img_tensor,axis=0)
    img_tensor /=255.

    prediction = model.predict(img_tensor)

    return prediction[0][0]