<script setup lang="ts">
defineProps<{ title: string; color: string; icon: string; }>();
</script>

<template>
  <li class="space-y-1 mb-4 rounded-md overflow-hidden" :class="`bg-${color}-100 dark:bg-${color}-800`">
    <button class="flex items-center p-2 w-full" :class="`bg-${color}-200 dark:bg-${color}-700`">
      <Icon Name="icon" CssClass="size-5 flex-none fill-slate-900 dark:fill-white" />
      <span class="flex-1 px-2">{{title}}</span>
      <svg class="size-5 flex-none fill-slate-900 dark:fill-white" viewBox="0 0 24 24"
           xmlns="http://www.w3.org/2000/svg">
        <g id="SVGRepo_bgCarrier" stroke-width="0"></g>
        <g id="SVGRepo_tracerCarrier" stroke-linecap="round" stroke-linejoin="round"></g>
        <g id="SVGRepo_iconCarrier">
          <path fill-rule="evenodd" clip-rule="evenodd"
                d="M4.43057 8.51192C4.70014 8.19743 5.17361 8.161 5.48811 8.43057L12 14.0122L18.5119 8.43057C18.8264 8.16101 19.2999 8.19743 19.5695 8.51192C19.839 8.82642 19.8026 9.29989 19.4881 9.56946L12.4881 15.5695C12.2072 15.8102 11.7928 15.8102 11.5119 15.5695L4.51192 9.56946C4.19743 9.29989 4.161 8.82641 4.43057 8.51192Z"></path>
        </g>
      </svg>
    </button>
    <ul class="p-2 space-y-2 pl-4">
      <slot></slot>
    </ul>
  </li>
</template>
