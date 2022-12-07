import { readLinesFromStdin, runSolutions } from "./utils/common.ts";

/**
 * Finds the zero-based index of the first character representing the "start-of-packet" marker.
 * @param buffer The data buffer.
 * @returns The zero-based index of the "start-of-packet" market.
 */
function indexOfPacketStart(buffer: string): number {
    for (let i = 0; i < buffer.length; i++) {
        // Read in the first four characters into the set, starting from "i". If our set isn't
        // exactly four characters, then we know that we haven't found the start-of-packet marker (which
        // is represented by four unique characters).
        const maybeMarker = buffer.substring(i, i + 4);
        const marker = new Set(maybeMarker);
        if (marker.size === 4) {
            // Found our marker! Return the index of the start (i.e., add four).
            return i + 4;
        }
    }

    return -1;
}

/**
 * Finds the zero-based index of the first character representing the "start-of-message" marker
 * right `indexOfStart`.
 * @param buffer The buffer.
 * @param indexOfStart The start of the buffer.
 */
function indexOfMessageStart(buffer: string, indexOfStart: number): number {
    for (let i = indexOfStart; i < buffer.length; i++) {
        // Read in the first fourteen characters into the set, starting from "i". If our set isn't
        // exactly fourteen characters, then we know that we haven't found the start-of-message marker 
        // (which is represented by fourteen unique characters).
        const maybeMarker = buffer.substring(i, i + 14);
        const marker = new Set(maybeMarker);
        if (marker.size === 14) {
            // Found our marker! Return the index of the start (i.e., add four).
            return i + 14;
        }
    }

    return -1;
}

runSolutions(
    async () => {
        // We're only expecting a single line. :)
        const buffer = await (await readLinesFromStdin(false).next()).value;

        return indexOfPacketStart(buffer);
    },
    async () => {
        // We're only expecting a single line. :)
        const buffer = await (await readLinesFromStdin(false).next()).value;
        const startIndex = indexOfPacketStart(buffer);

        return indexOfMessageStart(buffer, startIndex);
    });
