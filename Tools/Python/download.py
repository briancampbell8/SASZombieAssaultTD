import requests
import zipfile
import io
import os

url = "https://kenney.nl/content/kenney_assets.zip"
root = "Content"

os.makedirs(root, exist_ok=True)

print("Downloading Kenney EVERYTHING pack...")

response = requests.get(url)
zip_bytes = io.BytesIO(response.content)

print("Extracting...")

with zipfile.ZipFile(zip_bytes) as z:
    z.extractall(root)

print("Done!")