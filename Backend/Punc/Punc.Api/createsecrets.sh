#!/bin/bash

echo "\nGoogle maps geocode api key "
read geocode

echo "Google maps autocomplete api key "
read ackey

echo "Google maps directions api key "
read directionsKey

echo "Recaptcha api key "
read recaptcha

echo "Stripe client secret "
read stripe

kubectl create secret generic nowleave.googlemaps.geocode.apikey --namespace=nowleave --from-literal=apisecret=$geocode

kubectl create secret generic nowleave.googlemaps.autocomplete.apikey --namespace=nowleave --from-literal=apisecret=$ackey

kubectl create secret generic nowleave.googlemaps.directions.apikey --namespace=nowleave --from-literal=apisecret=$directionsKey

kubectl create secret generic nowleave.recaptcha.apikey --namespace=nowleave --from-literal=apisecret=$recaptcha

kubectl create secret generic nowleave.stripe.clientsecret --namespace=nowleave --from-literal=apisecret=$stripe

