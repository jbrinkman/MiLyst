<script setup lang="ts">
import { Button } from '@/components/ui/button'
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-background text-foreground">
    <div class="w-full max-w-xl px-6">
      <div class="rounded-lg border border-border bg-card p-6 shadow-sm">
        <h1 class="text-2xl font-semibold tracking-tight">MiLyst</h1>
        <p class="mt-2 text-sm text-muted-foreground">Modern task management. Coming soon.</p>

        <div class="mt-6">
          <Button variant="default">Get Started</Button>
        </div>
      </div>
    </div>
  </div>
</template>
