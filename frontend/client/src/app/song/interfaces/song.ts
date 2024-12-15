import { Track } from "./track";

export interface Song {
    artistName: string;
    title: string;
    tracks: Track[];
}