# Horizontal pod Auto scaler

This repository contains basic dot.net core API which exposes prometheus metrics using prometheus-net.AspNetCore and prometheus-net.DotNetRuntime nuget package at /metrics. And then the same metrics would be used for auto scaling the application


## Different components used for setting up HPA
* Docker file which is available in repo.
* image is also available at docker hub with the name vigneshwar11/hpaexample:v1
* Deployment manifest file is available in the repo.
* Keda scaler maifest file is also available in repo.
* Install docker desktop locally using this link https://docs.docker.com/desktop/install/windows-install/.
* Installing Minikube locally using link https://minikube.sigs.k8s.io/docs/start/.
* Installing Helm using https://helm.sh/docs/intro/install/ and if needed install Chocolatey as well for before installing helm.
* Installing Prometheus and Grafana into minikube cluster using helm chart.
* Installing K6 for Load testing to trigger HPA


## Setting up minikube cluster
* Open power shell or CMD and type
```minikube start```
* Type below command to know it works well
```kubectl get all```
* Install Prometheus on minikube cluster using helm with below commands
* Adding prmoetheus repository(Run power shell with admin).
``` helm repo add prometheus-community https://prometheus-community.github.io/helm-charts ```
* Installing Prometheus 
```helm install prometheus prometheus-community/prometheus```
* Expose the prometheus-server service via NodePort
``` kubectl expose service prometheus-server --type=NodePort --target-port=9090 --name=prometheus-server-np ```
* Check services and pod's are created by running
``` kubectl get all ```
* Expose prometheus service via url
``` minikube service prometheus-server-np --url ```
* if you get this error/warning  "Because you are using a Docker driver on windows, the terminal needs to be open to run it." then use below command(update port according to yur environment.)
```Kubectl port-forward service/prometheus-server-np 54679:80```
* Now try to open the URL which was shown on CMD in previous step on browser and you should get prometheus.
* Install grafana using below steps if needed but for this excersise its not needed(Run power shell with admin).

    ``` helm repo add grafana https://grafana.github.io/helm-charts ```

    ``` helm install grafana grafana/grafana ```

    ``` kubectl expose service grafana --type=NodePort --target-port=3000 --name=grafana-np ```

    ``` kubectl get secret --namespace default grafana -o jsonpath="{.data.admin-password}" ```
* Use any online tool to do Base64 decode to get the actual password from previous command output.

    ``` minikube service grafana-np --url ```
* Run below command with correct port and the use the output url to connect to grafana.

    ```Kubectl service grafana-np 54679:80```

* Then set up data source as prometheus in Grafana UI with prometheus URL "http:\\prometheus-server:80"

* Install Keda with below commands(Run power sheel with admin).
  
  ```helm repo add kedacore https://kedacore.github.io/charts ```

  ``` helm repo update ```
 
  ``` kubectl create namespace keda ```

  ``` helm install keda kedacore/keda --namespace keda ``` 

 * Use deployment.yaml manifest in the repo to deploy.

   ``` kubectl create -f deployment.yaml ``` 

* check your deployment and there should be one new pod and one new service should have been started.

    ``` kubectl get pod ```

    ``` kubectl get service ```

* Check Prometheus server end point

    ```  kubectl describe service/prometheus-server ```

* Look for IP and replace serverAddress in Keda-Prometheus-scalar.yaml and make sure prometheus is running and you are able to connect it to prometheus UI.

* Deploy Keda prometheus scaler using the file "Keda-Prometheus-scalar.yaml" in the repo
 
    ``` kubectl create -f Keda-Prometheus-scalar.yaml ```

* check the HPA and verify TARGETS is not UNKNOWN

    ``` kubectl get hpa ```

* Run the service associated with the application pod

    ``` minikube service api-service --url ```

* Enable path forwarding 

    ``` Kubectl port-forward service/api-service 52464:80 ```

* verify you are able to connect to API using postman or any tool of your liking using url "http://127.0.0.1:52464/Joke"(Replace IP and port according to your output of the previous command)

* Verify you could see metrics on which you are trying to scale(look for query in Keda-Prometheus-scalar.yaml) is available in prometheus

* Install k6 with this link https://k6.io/docs/get-started/installation/

* Then update loadtest.js with correct API url, in my case http://127.0.0.1:52464/Joke

* Run K6 load test from terminal using 

    ``` k6 run loadtest.js ```
    


 
