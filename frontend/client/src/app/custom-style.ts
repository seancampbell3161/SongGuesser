import { definePreset } from "@primeng/themes";
import Aura from "@primeng/themes/aura";

export const MyPreset = definePreset(Aura, {
    semantic: {
        primary: {
            50: '{indigo.50}',
            100: '{indigo.100}',
            200: '{indigo.200}',
            300: '{indigo.300}',
            400: '{indigo.400}',
            500: '{indigo.500}',
            600: '{indigo.600}',
            700: '{indigo.700}',
            800: '{indigo.800}',
            900: '{indigo.900}',
            950: '{indigo.950}'
        },
        colorScheme: {
            light: {
                primary: {
                    color: '{indigo.600}',
                    inverseColor: '#ffffff',
                    hoverColor: '{indigo.800}',
                    activeColor: '{indigo.800}',
                },
                highlight: {
                    background: '{indigo.750}',
                    focusBackground: '{indigo.500}',
                    color: '#ffffff',
                    focusColor: '#ffffff',
                },
            },
            dark: {
                primary: {
                    color: '{indigo.400}',
                    inverseColor: '{indigo.800}',
                    hoverColor: '{indigo.200}',
                    activeColor: '{indigo.300}',
                },
                highlight: {
                    background: 'rgba(250, 250, 250, .16)',
                    focusBackground: 'rgba(250, 250, 250, .24)',
                    color: 'rgba(255,255,255,.87)',
                    focusColor: 'rgba(255,255,255,.87)',
                },
            },
        }
    }
});