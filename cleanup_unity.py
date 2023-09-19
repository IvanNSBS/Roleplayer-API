import os

extensions_to_del = [".meta", ".asmdef", ".prefab"]

def invalid_file(file_name: str, extensions):
    for extension in extensions_to_del:
        if file_name.endswith(extension):
            return True
    return False

def is_empty_subfolder(folder):
    if not os.path.isdir(folder):
        return False
    
    if len(os.listdir(folder)) == 0:
        return True
    
    return False

def main():
    for path, subdirs, files in os.walk("."):
        for name in files:
            if not invalid_file(name, extensions_to_del):
                continue

            file = os.path.join(path, name)
            os.remove(file)
            print(f"removed file {file}")

            if is_empty_subfolder(path):
                os.rmdir(path)
                print(f"removed dir {path}")

if __name__ == "__main__":
    main()