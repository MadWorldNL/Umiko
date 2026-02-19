Can # DNS Configuration

For production deployments, configure the following A records on your DNS server pointing to the IP address of your Kubernetes cluster:

| Record | Type | Value |
|--------|------|-------|
| `umiko.example.com` | A | `<cluster-ip>` |
| `www.umiko.example.com` | A | `<cluster-ip>` |
| `admin.umiko.example.com` | A | `<cluster-ip>` |
| `api.umiko.example.com` | A | `<cluster-ip>` |
| `bus.umiko.example.com` | A | `<cluster-ip>` |
| `database.umiko.example.com` | A | `<cluster-ip>` |
| `bus-management.umiko.example.com` | A | `<cluster-ip>` |
| `grafana.umiko.example.com` | A | `<cluster-ip>` |

Replace `umiko.example.com` with your actual domain and `<cluster-ip>` with the public IP address of your Kubernetes node running Traefik.

Alternatively, you can create a single wildcard A record:

| Record | Type | Value |
|--------|------|-------|
| `*.umiko.example.com` | A | `<cluster-ip>` |
| `umiko.example.com` | A | `<cluster-ip>` |

The wildcard record covers all subdomains. The root domain still needs its own A record since wildcards do not match the apex domain.