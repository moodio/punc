#!/bin/bash

CONNECTION_STRING=${1?Error: Please enter CONNECTION_STRING}
kubectl create secret generic koios.users.connectionstring --from-literal=connectionstring=$CONNECTION_STRING