import { Track } from "./track";

export interface Song {
    songId: number;
    artistName: string;
    title: string;
    tracks: Track[];
}