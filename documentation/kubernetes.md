# Kubernetes
This guide sets up a local Kubernetes development environment using Docker Desktop, kubectl, and helm. It also includes steps to enable the Kubernetes Dashboard for visual cluster management.

## Development environment
### Activate Kubernetes in Docker Desktop
* Open Docker Desktop.
* Go to Settings > Kubernetes.
* Enable the checkbox: Enable Kubernetes.
* Wait for Kubernetes to start (you'll see a green light or similar status when ready).

### Install Required Tools
Make sure you have the following installed:
* [kubectl](https://kubernetes.io/docs/tasks/tools/) – Kubernetes command-line tool.
* [helm](https://helm.sh/docs/intro/install/) – Kubernetes package manager.

### Kubernetes Dashboard
Enable the Kubernetes Dashboard by installing [Headlamp](https://headlamp.dev/docs/latest/installation/desktop/):

#### Windows

Using **winget**:
```shell
winget install headlamp
```

Using **Chocolatey**:
```shell
choco install headlamp
```

Or download the `.exe` installer directly from the [latest release](https://github.com/kubernetes-sigs/headlamp/releases/latest).

#### macOS

Using **Homebrew** (recommended):
```shell
brew install --cask --no-quarantine headlamp
```

Or download the `.dmg` file from the [latest release](https://github.com/kubernetes-sigs/headlamp/releases/latest).

If macOS blocks the app from running, open a terminal and run:
```shell
xattr -dr com.apple.quarantine /Applications/Headlamp.app
```
After this, running the app should work.

#### Open the Dashboard
Launch Headlamp and select your local Docker Desktop Kubernetes cluster. The dashboard gives you a visual overview of your cluster resources, workloads, and namespaces.

### Install Traefik
Install Traefik as the ingress controller:
```shell
helm repo add traefik https://traefik.github.io/charts
helm repo update
helm install traefik traefik/traefik -n traefik --create-namespace
helm upgrade traefik traefik/traefik -n traefik --set-json 'providers.kubernetesIngress.namespaces=["umiko-development"]'
```

### Setup TLS with mkcert
Install [mkcert](https://github.com/FiloSottile/mkcert) and create locally-trusted certificates:
```shell
mkcert -install
mkcert umiko.dev "*.umiko.dev"
kubectl create secret tls umiko-tls \
  --cert=umiko.dev+1.pem \
  --key=umiko.dev+1-key.pem \
  -n umiko-development
```

### Configure Hosts File
Add the following entries to your hosts file so the local domains resolve to your machine:

**Windows**: `C:\Windows\System32\drivers\etc\hosts`
**macOS / Linux**: `/etc/hosts`

```
127.0.0.1       umiko.dev
127.0.0.1       www.umiko.dev
127.0.0.1       admin.umiko.dev
127.0.0.1       api.umiko.dev
127.0.0.1       bus.umiko.dev
127.0.0.1       database.umiko.dev
127.0.0.1       bus-management.umiko.dev
127.0.0.1       grafana.umiko.dev
```

### Deploy to Development
Navigate to the folder `deployment/umiko` and execute the commands below.

#### Install
```shell
helm install -f values.yaml -f values-development.yaml umiko .
```

#### Upgrade
```shell
helm upgrade -f values.yaml -f values-development.yaml umiko .
```

## Production
### Install on production
#### Step 1: Install MicroK8s
Execute this install command:
```shell
sudo snap install microk8s --classic
sudo microk8s status --wait-ready
```

#### Step 2: Enable services
Required:
```shell
sudo microk8s enable dns
sudo microk8s enable helm
sudo microk8s enable cert-manager
sudo microk8s enable hostpath-storage
```

Optional:
```shell
sudo microk8s enable metrics-server
sudo microk8s enable prometheus
```

#### Step 3: Install Traefik
Install Traefik as the ingress controller:
```shell
sudo microk8s helm repo add traefik https://traefik.github.io/charts
sudo microk8s helm repo update
sudo microk8s helm install traefik traefik/traefik -n traefik --create-namespace
sudo microk8s helm upgrade traefik traefik/traefik -n traefik \
  --set ports.web.hostPort=80 \
  --set ports.websecure.hostPort=443 \
  --set "additionalArguments={--entrypoints.web.http.redirections.entryPoint.to=:443,--entrypoints.web.http.redirections.entryPoint.scheme=https}" \
  --set deployment.strategy.type=Recreate
```

If the new Traefik pod is stuck in `Pending` after an upgrade, the old pod may still be holding ports 80/443. Delete it manually:
```shell
sudo microk8s kubectl delete pod <old-traefik-pod-name> -n traefik
```

### Usage on production
#### Step 1 - Download source code
```shell
git clone https://github.com/MadWorldNL/Umiko
```

#### Step 2 - Install or Upgrade Cluster
Navigate to the folder `deployment/umiko` and execute one of the commands below.

Install:
```shell
microk8s helm install -f values.yaml -f values-production.yaml -f values-secrets.yaml umiko .
```

Upgrade:
```shell
microk8s helm upgrade -f values.yaml -f values-production.yaml -f values-secrets.yaml umiko .
```

#### Step 3 - Install Headlamp
Install [Headlamp](https://headlamp.dev/) as the Kubernetes dashboard:
```shell
sudo microk8s helm repo add headlamp https://kubernetes-sigs.github.io/headlamp/
sudo microk8s helm repo update
sudo microk8s helm install my-headlamp headlamp/headlamp --namespace kube-system
```

#### Step 4 - Create Headlamp Token
Create a token for logging in to Headlamp:
```shell
sudo microk8s kubectl create token my-headlamp --namespace kube-system
```

#### Step 5 - Access Headlamp
Forward the Headlamp port to access the dashboard:
```shell
export POD_NAME=$(sudo microk8s kubectl get pods --namespace kube-system -l "app.kubernetes.io/name=headlamp,app.kubernetes.io/instance=my-headlamp" -o jsonpath="{.items[0].metadata.name}")
export CONTAINER_PORT=$(sudo microk8s kubectl get pod --namespace kube-system $POD_NAME -o jsonpath="{.spec.containers[0].ports[0].containerPort}")
sudo microk8s kubectl --namespace kube-system port-forward --address 0.0.0.0 $POD_NAME 10443:$CONTAINER_PORT
```

### Reference
[MicroK8s install guide](https://microk8s.io/)