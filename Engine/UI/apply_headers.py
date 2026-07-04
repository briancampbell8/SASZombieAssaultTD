import os
import json
import re

def run_simple_pipeline():
    json_path = "metadata.json"
    
    if not os.path.exists(json_path):
        print(f"[-] Error: '{json_path}' file is missing.")
        return

    with open(json_path, 'r', encoding='utf-8') as f:
        database = json.load(f)

    for file_name, data in database.items():
        if not os.path.exists(file_name):
            print(f"[!] File not found, skipping: {file_name}")
            continue

        action = data.get("engine_action", "").lower()
        if action == "skip":
            print(f"[→] Skipping: {file_name}")
            continue

        # 1. Read existing source file text
        with open(file_name, 'r', encoding='utf-8') as f_src:
            content = f_src.read()

        # 2. Overwrite Protection: Strip old headers if replacing
        if action == "overwrite":
            print(f"[⇄] Stripping old header from: {file_name}")
            content = re.sub(r'^//\s*=+.*?(//\s*=+[\r\n]+)', '', content, flags=re.DOTALL).lstrip()
        
        # 3. Add Protection: Skip if a standard block is already there
        elif action == "add" and ("FILE:" in content and "ROLE:" in content):
            print(f"[→] Header already exists in {file_name}, skipping.")
            continue

        # 4. Format the custom comment block
        header = [
            "// ====================================================================================================",
            f"//  FILE: {file_name}",
            f"//  PATH: ./Engine/UI/",
            f"//  MODULE: {data.get('moduleName', 'System Module')}",
            "//",
            "//  ROLE:",
            f"//      {data.get('role', 'Component class.')}",
            "//",
            "//  RESPONSIBILITIES:"
        ]
        for resp in data.get("responsibilities", []):
            header.append(f"//      - {resp}")
        header.append("//")
        header.append("//  NON-RESPONSIBILITIES:")
        header.append("//      - Low-level data persistence or file serialization.")
        header.append("//")
        header.append("//  NOTES:")
        header.append("//      Auto-generated structure verified locally via file state scripts.")
        header.append("// ====================================================================================================\n")
        
        final_block = "\n".join(header)

        # 5. Write the final block + file content back to disk
        with open(file_name, 'w', encoding='utf-8') as f_src:
            f_src.write(final_block + content)
        print(f"[✓] Successfully updated: {file_name}")

if __name__ == "__main__":
    run_simple_pipeline()
