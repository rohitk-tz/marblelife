# OWASP Top 10 Detection Patterns

## A01: Broken Access Control

```python
# VULNERABLE: No authorization check
@app.get("/api/users/{user_id}")
def get_user(user_id: int):
    return db.get_user(user_id)  # Any user can access any user's data

# SECURE: Authorization check
@app.get("/api/users/{user_id}")
def get_user(user_id: int, current_user: User = Depends(get_current_user)):
    if current_user.id != user_id and not current_user.is_admin:
        raise HTTPException(403, "Forbidden")
    return db.get_user(user_id)
```

## A02: Cryptographic Failures

```python
# VULNERABLE: MD5 for password
password_hash = hashlib.md5(password.encode()).hexdigest()

# SECURE: bcrypt
from passlib.hash import bcrypt
password_hash = bcrypt.hash(password)
```

## A03: Injection

```python
# SQL INJECTION - VULNERABLE:
query = f"SELECT * FROM users WHERE id = {user_id}"

# SQL INJECTION - SECURE:
cursor.execute("SELECT * FROM users WHERE id = %s", (user_id,))

# COMMAND INJECTION - VULNERABLE:
os.system(f"convert {user_file} output.png")

# COMMAND INJECTION - SECURE:
subprocess.run(["convert", user_file, "output.png"], check=True)

# TEMPLATE INJECTION (SSTI) - VULNERABLE:
Template(f"Hello {user_input}").render()

# TEMPLATE INJECTION - SECURE:
Template("Hello {{ name }}").render(name=user_input)
```

## A05: Security Misconfiguration

```python
# VULNERABLE: Debug mode in production
app.run(debug=True)

# SECURE: Proper security headers
app.config["SECURITY_HEADERS"] = {
    "X-Content-Type-Options": "nosniff",
    "X-Frame-Options": "DENY",
    "Content-Security-Policy": "default-src 'self'"
}
```

## A07: Authentication Failures

```python
# VULNERABLE: JWT with no verification
jwt.decode(token, options={"verify_signature": False})

# VULNERABLE: JWT with hardcoded symmetric key
jwt.decode(token, "hardcoded-secret", algorithms=["HS256"])

# SECURE: JWT with proper configuration
jwt.decode(token, get_secret_key(), algorithms=["RS256"])
```

## A08: Integrity Failures

```python
# VULNERABLE: Pickle with untrusted data
data = pickle.loads(user_input)  # RCE vulnerability

# VULNERABLE: YAML with arbitrary objects
data = yaml.load(user_input, Loader=yaml.Loader)

# SECURE: Safe deserialization
data = json.loads(user_input)
data = yaml.safe_load(user_input)
```

## A10: SSRF

```python
# VULNERABLE:
url = request.args.get("url")
response = requests.get(url)  # Can access internal services

# SECURE: URL validation
ALLOWED_HOSTS = ["api.example.com", "cdn.example.com"]
parsed = urllib.parse.urlparse(url)
if parsed.netloc not in ALLOWED_HOSTS:
    raise ValueError("Host not allowed")
```
