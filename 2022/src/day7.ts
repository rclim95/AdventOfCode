import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

function isFolder(item: File | Folder): item is Folder {
    return "parent" in item;
}

/**
 * Represents a file.
 */
class File {
    private _name: string;
    private _size: number;

    /**
     * Constructor.
     * @param name The name of the file.
     * @param size The size of the file.
     */
    constructor(name: string, size: number) {
        this._name = name;
        this._size = size;
    }

    /**
     * Gets the name of the file.
     */
    public get name() {
        return this._name;
    }

    /**
     * Gets the size of the file.
     */
    public get size() {
        return this._size;
    }
}

/**
 * Represents a folder, which is capable of holding additional folders and files.
 */
class Folder {
    private _name: string;
    private _items: Array<File | Folder>;
    private _parent: Folder | null;
    private _size: number;

    /**
     * Constructor.
     * @param name The name of the folder.
     */
    constructor(name: string) {
        this._parent = null;
        this._name = name;
        this._size = 0;
        this._items = [];
    }

    /**
     * Gets the name of this folder.
     */
    public get name() {
        return this._name;
    }

    /**
     * Gets the parent of this folder.
     */
    public get parent() {
        return this._parent;
    }

    /**
     * Gets the size of this folder.
     */
    public get size(): number {
        return this._size;
    }

    /**
     * Adds a file to this directory.
     * @param file The file to add.
     */
    public addFile(file: File) {
        this._items.push(file);
        
        // Update our sizes and our parents' size until we reach the root. :)
        // deno-lint-ignore no-this-alias
        let parent: Folder | null = this;
        while (parent !== null) {
            parent._size += file.size;
            parent = parent._parent;
        }
    }

    /**
     * Adds a folder to this directory.
     * @param folder The folder to add.
     */
    public addFolder(folder: Folder) {
        folder._parent = this;
        this._items.push(folder);
    }

    /**
     * Gets the `Folder` specified.
     * @param folderName  The name of the folder.
     * @returns The `Folder` instance matching the specified `folderName` or `null` if it doesn't exist.
     */
    public getFolder(folderName: string): Folder | null {
        for (const fileOrFolder of this._items) {
            if (!isFolder(fileOrFolder)) {
                continue;
            }

            if (fileOrFolder.name === folderName) {
                return fileOrFolder;
            }
        }

        return null;
    }

    /**
     * Gets the folder inside this folder.
     * @returns The folder sinside this folder.
     */
    public getFolders(): Array<Folder> {
        return this._items
            .filter((item): item is Folder => isFolder(item))
            .sort((folder1, folder2) => folder1.name.localeCompare(folder2.name));
    }

    /**
     * Gets the files inside this folder.
     * @returns The files inside this folder.
     */
    public getFiles(): Array<File> {
        return this._items
            .filter((item): item is File => !isFolder(item))
            .sort((file1, file2) => file1.name.localeCompare(file2.name));
    }
}

function printFolderStructure(root: Folder, level = 0): string {
    const prefix = " ".repeat(level);
    let output = `${prefix}- ${root.name} (dir, size=${root.size})\n`;

    // First list out the folders.
    for (const folder of root.getFolders()) {
        output += printFolderStructure(folder, level + 2);
    }

    // Then print out the files.
    for (const file of root.getFiles()) {
        output += `${prefix}  - ${file.name} (file, size=${file.size})\n`;
    }

    return output;
}

async function buildFileSystem() {
    const root = new Folder("/");
    let currentFolder: Folder | null = root;
    let parsingLsOutput = false;
    for await (const line of readLinesFromStdin(false)) {
        // Are we parsing a command?
        if (line.startsWith("$")) {
            // If we're here, then we know we aren't parsing the output of `$ ls` anymore. :)
            parsingLsOutput = false;

            // This is a command. Now figure out which command we're running using regular expression.
            const tokens = line.substring(2).split(" ");
            switch (tokens[0]) {
                case "cd": {
                        // Changing our current folder. Figure out where we're going.
                        if (tokens[1] === "/") {
                            // Go back to root.
                            currentFolder = root;
                        }
                        else if (tokens[1] === "..") {
                            // Go up one directory.
                            currentFolder = currentFolder?.parent ?? null;
                        }
                        else {
                            // Go into the folder that the user has requested.
                            const maybeFolder: Folder | null = currentFolder?.getFolder(tokens[1]) ?? null;
                            if (maybeFolder === null) {
                                throw new Error("Unable to find folder \"" + tokens[1] + "\" in current directory.");
                            }

                            currentFolder = maybeFolder;
                        }
                    }
                    break;
                
                case "ls":
                    // Listing files and folders in the current folder.
                    parsingLsOutput = true;
                    continue;
            }
        }
        else if (parsingLsOutput) {
            // Sanity check--make sure we have a current folder!
            if (currentFolder === null) {
                throw new Error("Unable to create file or folder because currentFolder is null");
            }

            // We're not executing a command--we're parsing the output of `$ ls`.
            //
            // Now figure out if we're parsing a directory or a file info. :)
            const tokens = line.split(" ");
            if (tokens[0] === "dir") {
                // This is a directory. Create it now.
                currentFolder?.addFolder(new Folder(tokens[1]));
            }
            else {
                // This is a file. Create it now.
                currentFolder?.addFile(new File(tokens[1], parseInt(tokens[0])))
            }
        }
    }

    return root;
}

async function sumOfLargeDirectories() {
    // Read in our file system based off our terminal inputs, and print out the structure for fun.
    const rootFolder = await buildFileSystem();
    console.debug(printFolderStructure(rootFolder));

    // Now go through the tree and collect all the directories whose sizes are greater than 100,000.
    // To iterate through the tree, we'll do a depth-first search in an iterative form.
    const sumApplicableFolder = (root: Folder): number => {
        let sum = 0;
        if (root.size <= 100000) {
            sum += root.size;
        }

        for (const folder of root.getFolders()) {
            sum += sumApplicableFolder(folder);
        }

        return sum;
    }
    return sumApplicableFolder(rootFolder);
}

async function sizeOfSmallestDirectoryToDelete() {
    const TotalDiskSpace = 70_000_000;
    const FreeSpaceRequired = 30_000_000;

    // Read in our file system based off our terminal inputs, and print out the structure for fun.
    const rootFolder = await buildFileSystem();
    const availableFreeSpace = TotalDiskSpace - rootFolder.size;
    const freeSpaceNeeded = FreeSpaceRequired - availableFreeSpace;
    console.debug(printFolderStructure(rootFolder));
    console.debug("Free Space Required:", FreeSpaceRequired);
    console.debug("Available Free Space:", availableFreeSpace);

    // Traverse through the tree and figure out which directory is the "smallest", starting from
    // the current available free space.
    const findSmallestFolder = (root: Folder, currentMinimum: number): number => {
        // Only consider folders that are greater than the available free space. Otherwise,
        // we'll ignore them.Figuri
        if (root.size >= freeSpaceNeeded) {
            currentMinimum = Math.min(currentMinimum, root.size);
        }
        
        for (const folder of root.getFolders()) {
            if (folder.size) 
            currentMinimum = Math.min(currentMinimum, findSmallestFolder(folder, currentMinimum));
        }

        return currentMinimum;
    };

    return findSmallestFolder(rootFolder, TotalDiskSpace);
}

runSolutions(sumOfLargeDirectories, sizeOfSmallestDirectoryToDelete);
