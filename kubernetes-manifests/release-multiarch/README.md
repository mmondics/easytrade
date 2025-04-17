# release-multiarch

This directory contains YAML files that will deploy easytrade onto either an IBM LinuxONE (s390x architecture) or Intel/x86 (amd64) architecture OpenShift or Kubernetes cluster. 

At the time of deployment, Kubernetes will pull & run the proper architecture container image from the manifest file on the compute node.

Follow the instructions in the easytrade [README](../../README.md#openshift-instructions) making sure to specify the `release-multiarch` directory.

This `release-multiarch` version does not include the original MSSQL database as it is not supported on s390x.  