<script setup lang="ts">
import type { DefineComponent } from 'vue';
import { MenuDownIcon } from 'mdi-vue3';

defineProps<{ title: string; color: string; icon: DefineComponent }>();
</script>

<template>
  <li
    class="space-y-1 mb-4 rounded-md overflow-hidden"
    :class="`bg-${color}-100 dark:bg-${color}-800`"
  >
    <button class="flex items-center p-2 w-full" :class="`bg-${color}-200 dark:bg-${color}-700`">
      <component :is="icon" class="size-5 flex-none fill-slate-900 dark:fill-white" />
      <span class="flex-1 px-2">{{ title }}</span>
      <menu-down-icon class="size-5 flex-none fill-slate-900 dark:fill-white" />
    </button>
    <ul class="p-2 space-y-2 pl-4">
      <slot></slot>
    </ul>
  </li>
</template>
