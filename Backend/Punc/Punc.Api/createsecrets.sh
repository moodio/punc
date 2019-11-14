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

echo "Route53 IAM User for certmanager key:"
read certmanagerKey

echo "Route53 IAM user for cermanager secret:"
read certmanagerSecret

echo "Database connection string:"
read connectionString

echo "AWS SES access key secret"
read awsses


kubectl create secret generic nowleave.googlemaps.geocode.apikey --namespace=nowleave --from-literal=apisecret=$geocode

kubectl create secret generic nowleave.googlemaps.autocomplete.apikey --namespace=nowleave --from-literal=apisecret=$ackey

kubectl create secret generic nowleave.googlemaps.directions.apikey --namespace=nowleave --from-literal=apisecret=$directionsKey

kubectl create secret generic nowleave.recaptcha.apikey --namespace=nowleave --from-literal=apisecret=$recaptcha

kubectl create secret generic nowleave.stripe.clientsecret --namespace=nowleave --from-literal=apisecret=$stripe

kubectl create secret generic nowleave.certmanager.route53 --namespace=cert-manager --from-literal=secret-access-key=$certmanagerSecret

kubectl create secret generic nowleave.database.connectionstring --namespace=nowleave --from-literal=connectionstring=$connectionString

kubectl create secret generic nowleave.health.echo -n nowleave --from-literal=apisecret=hellothere

kubectl create secret generic nowleave.aws.ses.accesskey -n nowleave --from-literal=secret=$awsses

kubectl create secret generic nowleave.sendgrid.apikey -n nowleave --from-literal=apikey=$sendgridkey