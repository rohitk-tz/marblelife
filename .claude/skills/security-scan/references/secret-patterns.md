# Secret Detection Patterns

## Regex patterns for scanning

```regex
# API Keys (General)
["\']?[aA][pP][iI][_-]?[kK][eE][yY]["']?\s*[:=]\s*["'][a-zA-Z0-9_-]{20,}["']

# AWS
AKIA[0-9A-Z]{16}
aws[_-]?secret[_-]?access[_-]?key\s*[:=]\s*["\'][a-zA-Z0-9/+=]{40}["\']

# Google Cloud
AIza[0-9A-Za-z_-]{35}

# GitHub
gh[pousr]_[a-zA-Z0-9]{36}
github[_-]?token\s*[:=]\s*["\'][a-zA-Z0-9_-]{40}["\']

# Stripe
sk_live_[0-9a-zA-Z]{24}
rk_live_[0-9a-zA-Z]{24}

# Slack
xox[baprs]-[0-9]{10,13}-[0-9]{10,13}-[a-zA-Z0-9]{24}

# JWT Tokens
eyJ[a-zA-Z0-9_-]*\.eyJ[a-zA-Z0-9_-]*\.[a-zA-Z0-9_-]*

# Private Keys
-----BEGIN (RSA |DSA |EC |OPENSSH )?PRIVATE KEY-----

# Database URLs
(postgres|mysql|mongodb|redis)://[^"\s]+:[^"\s]+@[^"\s]+

# Generic Passwords/Secrets
["\']?[pP]assword["']?\s*[:=]\s*["'][^"'\s]{8,}["']
["\']?[sS]ecret["']?\s*[:=]\s*["'][^"'\s]{8,}["']
```
