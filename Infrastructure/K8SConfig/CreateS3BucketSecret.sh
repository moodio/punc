#!/bin/bash

API_SECRET=${1?Error: Please enter API_SECRET}
kubectl create secret generic koios.services.imageupload.s3bucket --from-literal=apisecret=$API_SECRET