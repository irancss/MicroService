'use client';
import { motion } from 'framer-motion';

const AnimatedDiv = ({ children, ...props }) => (
  <motion.div {...props}>{children}</motion.div>
);

export default AnimatedDiv;
