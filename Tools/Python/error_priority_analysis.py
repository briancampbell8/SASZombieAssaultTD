import re

def analyze_build_errors():
    """Analyze build errors and create priority listing"""
    try:
        with open('build_output.txt', 'r') as f:
            content = f.read()
    except FileNotFoundError:
        print("Error: build_output.txt not found")
        return
    
    # Extract error codes and count occurrences
    error_pattern = r'error (CS\d+)'
    errors = re.findall(error_pattern, content)
    
    if not errors:
        print("No compilation errors found!")
        return
    
    # Count errors by type
    error_counts = {}
    for error in errors:
        error_counts[error] = error_counts.get(error, 0) + 1
    
    # Sort by count (descending)
    sorted_errors = sorted(error_counts.items(), key=lambda x: x[1], reverse=True)
    
    print("COMPILATION ERROR PRIORITY LISTING")
    print("=" * 50)
    print(f"Total Errors: {len(errors)}")
    print("=" * 50)
    
    for i, (error_code, count) in enumerate(sorted_errors, 1):
        print(f"{i:2d}. {error_code}: {count:3d} occurrences")
    
    print("\n" + "=" * 50)
    print("PRIORITY ORDER (Fix from top to bottom)")
    print("=" * 50)
    
    for i, (error_code, count) in enumerate(sorted_errors, 1):
        print(f"{i:2d}. {error_code}: {count:3d} occurrences")

if __name__ == "__main__":
    analyze_build_errors()
