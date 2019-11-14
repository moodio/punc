
# Punc source code
Punc, the project code name for [NowLeave.com](https://www.nowleave.com), is a technical exercise by [Moodio](https://www.moodio.co.uk) for our sister company [Muljin](https://www.muljin.sg) on the use of several technologies and on our moving from Azure and Digital Ocean to a purely AWS environment.
Punc is a secure production grade environment. While not all features are complete (list of what remains can be found below), the main pieces are all there and should help anyone looking on how to get started.
The main technologies used in NowLeave are:

Infrastructure:
 - [Kubernetes](https://kubernetes.io/) 
 - Docker 
 - [Istio](https://istio.io/) 
 - [Cert Manager within Istio using DNS validation](https://docs.cert-manager.io/en/latest/)

Apis:
 - [Google Maps routes and autocomplete](https://developers.google.com/maps/documentation/javascript/places-autocomplete)
 - [Recaptcha V3](https://developers.google.com/recaptcha/docs/v3)
 - [Stripe API with SCA enabled](https://stripe.com)


PLEASE REMEMBER this is a technical exercise and a continuing work in progress. Its likely there are things we've forgotten to include in code. If you come across any issues however please raise an issue and we'll try to correct it as soon as possible. Please also remember that the purpose of this repo is to serve as an example rather than to work out the box, you'll still need to go through the documentation on your own cloud provider, as well as the document websites for the different technologies used, especially kubernetes and istio which are quite complex.

## Remaining Features
The main remaining features revolve around telemetry, logging and mutual tls between services, which will be added over the next few weeks to take advantage of the features made available by Istio. 
Additionally, should time permit, an example [gRPC]() service will be implemented and tracing.

Complete list is:

 - Seperation of dev and production environments 
 - Centralised logging
 - Tracing 
 - Telemetry 
 - gRPC Service 
 - Mutual TLS

If theres anything you'd like to see an example of, feel free to raise an issue and we'll do our best to include it.

## Running
Unfortunately we cannot give complete instructions on how to setup the kubernetes cluster, secrets, etc, as all of that is explained in detail on the kubernetes website.
However, please take note of the following:
 - Our experiment is run on AWS, with the domain DNS being managed on Route 53. If your setup is different, you will need to make changes to the Infrastructure\letsencrypt-issuer-prod.yaml file based on your own DNS provider. More specific instructions are available on the [Cert Manager Docs Website](https://docs.cert-manager.io/en/latest/tasks/issuers/setup-acme/dns01/index.html#id1)
 - When installing Istio, ensure that SDS is enabled and SDS is enabled, however that mTLS is DISABLED. A helm installation yaml will be added shortly however it's currently unavailable. Details on how to do this can be found on the istio website in anycase.
