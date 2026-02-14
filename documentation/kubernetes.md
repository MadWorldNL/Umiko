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
sudo microk8s enable dashboard
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
```

### Usage on production
#### Step 1 - Download source code
```shell
git clone https://github.com/MadWorldNL/MadTransfer
```

#### Step 2 - Install Storage
```shell
kubectl apply -f https://raw.githubusercontent.com/rancher/local-path-provisioner/master/deploy/local-path-storage.yaml
```

#### Step 3 - Install Cluster
Navigate to the folder `deployment/umiko` and execute this command:
```shell
microk8s helm install -f values.yaml -f values-production.yaml umiko .
```

#### Step 4 - Status of Cluster
Convenient tools for debugging Kubernetes:
```shell
microk8s dashboard-proxy --address 0.0.0.0
```

### Reference
[MicroK8s install guide](https://microk8s.io/)