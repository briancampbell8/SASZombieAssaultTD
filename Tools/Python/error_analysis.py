import re
import subprocess
import sys

def get_build_errors():
    """Get compilation errors and count by error type"""
    try:
        result = subprocess.run(
            ['dotnet', 'build', '--no-restore', '--verbosity', 'normal'],
            capture_output=True,
            text=True,
            cwd='.'
        )
        
        errors = []
        for line in result.stderr.split('\n'):
            if 'error CS' in line:
                # Extract error code
                match = re.search(r'error (CS\d+)', line)
                if match:
                    error_code = match.group(1)
                    errors.append(error_code)
        
        # Count errors by type
        error_counts = {}
        for error in errors:
            error_counts[error] = error_counts.get(error, 0) + 1
        
        # Sort by count (descending)
        sorted_errors = sorted(error_counts.items(), key=lambda x: x[1], reverse=True)
        
        return sorted_errors, len(errors)
        
    except Exception as e:
        print(f"Error running build: {e}")
        return [], 0

if __name__ == "__main__":
    errors, total = get_build_errors()
    
    print(f"COMPILATION ERROR PRIORITY LISTING")
    print(f"=" * 50)
    print(f"Total Errors: {total}")
    print(f"=" * 50)
    
    for i, (error_code, count) in enumerate(errors, 1):
        print(f"{i:2d}. {error_code}: {count:3d} occurrences")
    
    print(f"\n" + "=" * 50)
    print("PRIORITY ORDER (Fix from top to bottom)")
    print("=" * 50)
