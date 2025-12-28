import { defineConfig } from 'unocss'
import presetWind4 from '@unocss/preset-wind4'
import presetAnimations from 'unocss-preset-animations'
import { presetShadcn } from 'unocss-preset-shadcn'
import transformerDirectives from '@unocss/transformer-directives'
import transformerVariantGroup from '@unocss/transformer-variant-group'

export default defineConfig({
  presets: [
    presetWind4({
      preflights: true,
    }),
    presetAnimations(),
    presetShadcn(
      {
        color: 'neutral',
      },
      {
        componentLibrary: 'reka',
      },
    ),
  ],
  transformers: [transformerDirectives(), transformerVariantGroup()],
  content: {
    pipeline: {
      include: [
        /\.(vue|[jt]sx?|mdx?|astro|html)($|\?)/,
        '(components|src)/**/*.{js,ts}',
      ],
    },
  },
})
