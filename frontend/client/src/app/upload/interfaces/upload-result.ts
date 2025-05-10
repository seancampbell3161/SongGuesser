import { Track } from "../../song/interfaces/track";

export interface UploadResult {
    message: string | null;
    error: string | null;
    tracks: Track[];
}